// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace WDAC_Wizard
{
    /// <summary>
    /// Loads one or more App Control policies from disk and compares them, producing a hierarchical
    /// list of differences grouped by section (metadata, rule options, file rules, signers, etc.).
    /// </summary>
    internal static class PolicyComparer
    {
        /// <summary>
        /// Canonical ordering of comparison sections. Sections not in this list are treated as
        /// having a sort key larger than any listed section and are sorted alphabetically among
        /// themselves at the end. Centralized here so the renderer, exporter, and summary header
        /// all agree on order.
        /// </summary>
        public static readonly string[] SectionOrder = new[]
        {
            "Rule Options",
            "Policy Metadata",
            "Settings",
            "Signing Scenarios",
            "Signers",
            "Update Policy Signers",
            "Supplemental Policy Signers",
            "CI Signers",
            "EKUs",
            "File Rules",
        };

        /// <summary>
        /// Returns the canonical sort key for a section. Lower numbers sort first; sections not in
        /// the canonical list return int.MaxValue so they fall to the end (still alphabetical
        /// among themselves when ThenBy is applied).
        /// </summary>
        public static int OrderOf(string section)
        {
            if (section == null) return int.MaxValue;
            int idx = Array.IndexOf(SectionOrder, section);
            return idx < 0 ? int.MaxValue : idx;
        }

        /// <summary>
        /// Represents a single policy that has been loaded from disk and prepared for comparison.
        /// </summary>
        public class LoadedPolicy
        {
            public string SourcePath { get; set; }
            public string DisplayName { get; set; }
            public SiPolicy Policy { get; set; }
            public string LoadError { get; set; }

            // File metadata for traceability and reports
            public long FileSizeBytes { get; set; }
            public DateTime LastWriteUtc { get; set; }
            public string Sha256 { get; set; }
        }

        /// <summary>
        /// A single comparison entry: identifies an item (e.g. a rule option, file rule, signer) and which
        /// policies contain it. The Values dictionary stores per-policy display strings. If a policy does
        /// not contain the item, its value is null.
        /// </summary>
        public class ComparisonEntry
        {
            public string Section { get; set; }     // e.g. "Rule Options", "File Rules"
            public string Key { get; set; }         // unique within section
            public string DisplayName { get; set; } // human-readable name shown in UI
            public Dictionary<string, string> Values { get; set; } // policyDisplayName -> value (null if missing)
            public bool IsDifferent { get; set; }   // true if not all policies have the same value

            public ComparisonEntry()
            {
                Values = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Result of a multi-policy comparison: list of policies that were compared and a list of
        /// every entry seen in any of the policies for each section.
        /// </summary>
        public class ComparisonResult
        {
            public List<LoadedPolicy> Policies { get; set; } = new List<LoadedPolicy>();
            public List<ComparisonEntry> Entries { get; set; } = new List<ComparisonEntry>();
        }

        /// <summary>
        /// Loads a policy file from disk. Supports both XML policies and binary (.cip/.p7b) policies.
        /// </summary>
        /// <param name="policyPath">Path to the policy file</param>
        /// <returns>LoadedPolicy with either a populated Policy or a LoadError</returns>
        public static LoadedPolicy LoadPolicy(string policyPath)
        {
            var loaded = new LoadedPolicy
            {
                SourcePath = policyPath,
                DisplayName = string.IsNullOrEmpty(policyPath) ? "<unknown>" : Path.GetFileName(policyPath)
            };

            if (string.IsNullOrEmpty(policyPath) || !File.Exists(policyPath))
            {
                loaded.LoadError = "File not found.";
                return loaded;
            }

            try
            {
                // Capture file metadata up front (best-effort). These are useful in the report
                // even if parsing later fails.
                try
                {
                    var fi = new FileInfo(policyPath);
                    loaded.FileSizeBytes = fi.Length;
                    loaded.LastWriteUtc = fi.LastWriteTimeUtc;
                    loaded.Sha256 = ComputeSha256(policyPath);
                }
                catch (Exception metaEx)
                {
                    Logger.Log?.AddWarningMsg("PolicyComparer.LoadPolicy: failed to read file metadata: " + metaEx.Message);
                }

                string xmlPath = policyPath;

                // Convert binary policies to XML on disk first, then deserialize
                if (Helper.IsBinaryPolicyFile(policyPath))
                {
                    xmlPath = BinaryPolicyConverter.ConvertToXml(policyPath);
                    if (string.IsNullOrEmpty(xmlPath))
                    {
                        loaded.LoadError = "Unable to convert binary policy to XML.";
                        return loaded;
                    }
                }

                SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(xmlPath);
                if (siPolicy == null)
                {
                    loaded.LoadError = "Unable to parse policy XML.";
                    return loaded;
                }

                loaded.Policy = siPolicy;
            }
            catch (Exception ex)
            {
                Logger.Log?.AddErrorMsg("PolicyComparer.LoadPolicy caught the following exception", ex);
                loaded.LoadError = ex.Message;
            }

            return loaded;
        }

        private static string ComputeSha256(string path)
        {
            using (var stream = File.OpenRead(path))
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Compares the provided list of policies and returns a ComparisonResult containing per-section
        /// entries with the value from each policy.
        /// </summary>
        public static ComparisonResult Compare(List<LoadedPolicy> policies)
        {
            var result = new ComparisonResult();
            if (policies == null || policies.Count == 0)
            {
                return result;
            }

            // Ensure unique display names so duplicate file names do not collide as dictionary keys
            EnsureUniqueDisplayNames(policies);
            result.Policies.AddRange(policies);

            // Only operate on policies that loaded successfully
            var validPolicies = policies.Where(p => p.Policy != null).ToList();

            CompareMetadata(validPolicies, result);
            CompareRuleOptions(validPolicies, result);
            CompareFileRules(validPolicies, result);
            CompareSigners(validPolicies, result);
            CompareSigningScenarios(validPolicies, result);
            CompareUpdateSigners(validPolicies, result);
            CompareSupplementalSigners(validPolicies, result);
            CompareCiSigners(validPolicies, result);
            CompareSettings(validPolicies, result);
            CompareEKUs(validPolicies, result);

            // Mark which entries are different across policies
            foreach (var entry in result.Entries)
            {
                entry.IsDifferent = ComputeIsDifferent(entry, validPolicies);
            }

            return result;
        }

        private static void EnsureUniqueDisplayNames(List<LoadedPolicy> policies)
        {
            var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in policies)
            {
                string baseName = string.IsNullOrEmpty(p.DisplayName) ? "policy" : p.DisplayName;
                if (!seen.ContainsKey(baseName))
                {
                    seen[baseName] = 1;
                    p.DisplayName = baseName;
                }
                else
                {
                    seen[baseName]++;
                    p.DisplayName = string.Format("{0} ({1})", baseName, seen[baseName]);
                }
            }
        }

        private static bool ComputeIsDifferent(ComparisonEntry entry, List<LoadedPolicy> validPolicies)
        {
            // An entry is "different" if any valid policy has a value that differs from any other,
            // including the case where some policies are missing the item entirely.
            string firstValue = null;
            bool firstSet = false;
            foreach (var p in validPolicies)
            {
                entry.Values.TryGetValue(p.DisplayName, out string v);
                if (!firstSet)
                {
                    firstValue = v;
                    firstSet = true;
                }
                else if (!string.Equals(firstValue, v, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static void AddOrUpdateEntry(ComparisonResult result,
                                             string section,
                                             string key,
                                             string displayName,
                                             string policyName,
                                             string value)
        {
            var entry = result.Entries.FirstOrDefault(e => e.Section == section && e.Key == key);
            if (entry == null)
            {
                entry = new ComparisonEntry
                {
                    Section = section,
                    Key = key,
                    DisplayName = displayName,
                };
                result.Entries.Add(entry);
            }

            // If the same key was already added for this policy, prefer the latest non-null value
            entry.Values[policyName] = value;
        }

        private static void CompareMetadata(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Policy Metadata";
            foreach (var p in policies)
            {
                AddOrUpdateEntry(result, section, "FriendlyName", "Friendly Name",
                                 p.DisplayName, p.Policy.FriendlyName);
                AddOrUpdateEntry(result, section, "PolicyType", "Policy Type",
                                 p.DisplayName,
                                 p.Policy.PolicyTypeSpecified ? p.Policy.PolicyType.ToString() : null);
                AddOrUpdateEntry(result, section, "PolicyID", "Policy ID",
                                 p.DisplayName, p.Policy.PolicyID);
                AddOrUpdateEntry(result, section, "BasePolicyID", "Base Policy ID",
                                 p.DisplayName, p.Policy.BasePolicyID);
                AddOrUpdateEntry(result, section, "PolicyTypeID", "Policy Type ID",
                                 p.DisplayName, p.Policy.PolicyTypeID);
                AddOrUpdateEntry(result, section, "VersionEx", "Version",
                                 p.DisplayName, p.Policy.VersionEx);
                AddOrUpdateEntry(result, section, "PlatformID", "Platform ID",
                                 p.DisplayName, p.Policy.PlatformID);
                AddOrUpdateEntry(result, section, "HvciOptions", "HVCI Options",
                                 p.DisplayName,
                                 p.Policy.HvciOptionsSpecified ? p.Policy.HvciOptions.ToString() : null);
            }
        }

        private static void CompareRuleOptions(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Rule Options";

            // Pre-compute friendly name for every known OptionType so the user always sees
            // every option, even if no policy has it enabled.
            var optionTypeToFriendly = GetOptionTypeFriendlyNames();

            // First, ensure every option appears as a row, defaulting to "No" for each policy.
            foreach (var kvp in optionTypeToFriendly.OrderBy(k => k.Value, StringComparer.OrdinalIgnoreCase))
            {
                foreach (var p in policies)
                {
                    AddOrUpdateEntry(result, section, kvp.Key.ToString(), kvp.Value, p.DisplayName, "No");
                }
            }

            // Now mark each option present on a policy as "Yes".
            foreach (var p in policies)
            {
                if (p.Policy.Rules == null) continue;
                foreach (var rule in p.Policy.Rules)
                {
                    if (rule == null) continue;
                    string key = rule.Item.ToString();
                    string display = optionTypeToFriendly.TryGetValue(rule.Item, out string friendly)
                        ? friendly
                        : key;
                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, "Yes");
                }
            }
        }

        /// <summary>
        /// Builds a map from each OptionType enum value to its XML friendly name (e.g.
        /// OptionType.EnabledUMCI -> "Enabled:UMCI"). Falls back to the enum name when no
        /// XmlEnumAttribute is present.
        /// </summary>
        private static Dictionary<OptionType, string> GetOptionTypeFriendlyNames()
        {
            var map = new Dictionary<OptionType, string>();
            Type t = typeof(OptionType);
            foreach (OptionType value in Enum.GetValues(t))
            {
                FieldInfo field = t.GetField(value.ToString());
                string friendly = value.ToString();
                if (field != null)
                {
                    var attr = field.GetCustomAttribute<XmlEnumAttribute>();
                    if (attr != null && !string.IsNullOrEmpty(attr.Name))
                    {
                        friendly = attr.Name;
                    }
                }
                map[value] = friendly;
            }
            return map;
        }

        private static void CompareFileRules(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "File Rules";
            foreach (var p in policies)
            {
                if (p.Policy.FileRules == null) continue;
                foreach (var rule in p.Policy.FileRules)
                {
                    if (rule == null) continue;

                    string id = null;
                    string friendly = null;
                    string typeName = rule.GetType().Name;
                    string description = null;

                    switch (rule)
                    {
                        case Allow a:
                            id = a.ID;
                            friendly = a.FriendlyName;
                            description = BuildFileRuleDescription("Allow", a.FilePath, a.FileName,
                                                                   a.PackageFamilyName, a.MinimumFileVersion,
                                                                   a.MaximumFileVersion, a.Hash);
                            break;
                        case Deny d:
                            id = d.ID;
                            friendly = d.FriendlyName;
                            description = BuildFileRuleDescription("Deny", d.FilePath, d.FileName,
                                                                   d.PackageFamilyName, d.MinimumFileVersion,
                                                                   d.MaximumFileVersion, d.Hash);
                            break;
                        case FileAttrib fa:
                            id = fa.ID;
                            friendly = fa.FriendlyName;
                            description = BuildFileRuleDescription("FileAttrib", fa.FilePath, fa.FileName,
                                                                   fa.PackageFamilyName, fa.MinimumFileVersion,
                                                                   fa.MaximumFileVersion, fa.Hash);
                            break;
                        case FileRule fr:
                            id = fr.ID;
                            friendly = fr.FriendlyName;
                            description = BuildFileRuleDescription(fr.Type.ToString(), fr.FilePath,
                                                                   fr.FileName, fr.PackageFamilyName,
                                                                   fr.MinimumFileVersion, fr.MaximumFileVersion,
                                                                   fr.Hash);
                            break;
                    }

                    string key = string.IsNullOrEmpty(id)
                        ? string.Format("{0}|{1}", typeName, friendly ?? description ?? Guid.NewGuid().ToString())
                        : id;
                    string display = string.IsNullOrEmpty(friendly) ? (id ?? typeName) : friendly;

                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, description);
                }
            }
        }

        private static string BuildFileRuleDescription(string ruleType, string filePath, string fileName,
                                                       string packageFamilyName, string minimumVersion,
                                                       string maximumVersion, byte[] hash)
        {
            var parts = new List<string> { ruleType };
            if (!string.IsNullOrEmpty(filePath)) parts.Add("Path=" + filePath);
            if (!string.IsNullOrEmpty(fileName)) parts.Add("FileName=" + fileName);
            if (!string.IsNullOrEmpty(packageFamilyName)) parts.Add("PFN=" + packageFamilyName);
            if (!string.IsNullOrEmpty(minimumVersion)) parts.Add("MinVer=" + minimumVersion);
            if (!string.IsNullOrEmpty(maximumVersion)) parts.Add("MaxVer=" + maximumVersion);
            if (hash != null && hash.Length > 0)
            {
                parts.Add("Hash=" + BitConverter.ToString(hash).Replace("-", string.Empty));
            }
            return string.Join("; ", parts);
        }

        private static void CompareSigners(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Signers";
            foreach (var p in policies)
            {
                if (p.Policy.Signers == null) continue;
                foreach (var signer in p.Policy.Signers)
                {
                    if (signer == null) continue;
                    string key = string.IsNullOrEmpty(signer.ID)
                        ? "Signer|" + (signer.Name ?? Guid.NewGuid().ToString())
                        : signer.ID;
                    string display = string.IsNullOrEmpty(signer.Name) ? (signer.ID ?? "Signer") : signer.Name;
                    string description = BuildSignerDescription(signer);
                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, description);
                }
            }
        }

        private static string BuildSignerDescription(Signer signer)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(signer.Name)) parts.Add("Name=" + signer.Name);
            if (signer.CertRoot != null && signer.CertRoot.Value != null && signer.CertRoot.Value.Length > 0)
            {
                parts.Add("CertRoot=" + BitConverter.ToString(signer.CertRoot.Value).Replace("-", string.Empty));
            }
            if (signer.CertPublisher != null && !string.IsNullOrEmpty(signer.CertPublisher.Value))
            {
                parts.Add("Publisher=" + signer.CertPublisher.Value);
            }
            if (signer.CertIssuer != null && !string.IsNullOrEmpty(signer.CertIssuer.Value))
            {
                parts.Add("Issuer=" + signer.CertIssuer.Value);
            }
            if (signer.FileAttribRef != null && signer.FileAttribRef.Length > 0)
            {
                parts.Add("FileAttribRefs=" + signer.FileAttribRef.Length);
            }
            if (signer.SignTimeAfterSpecified)
            {
                parts.Add("SignTimeAfter=" + signer.SignTimeAfter.ToString("o"));
            }
            return parts.Count == 0 ? "Signer" : string.Join("; ", parts);
        }

        private static void CompareSigningScenarios(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Signing Scenarios";
            foreach (var p in policies)
            {
                if (p.Policy.SigningScenarios == null) continue;
                foreach (var scenario in p.Policy.SigningScenarios)
                {
                    if (scenario == null) continue;
                    string scenarioLabel = ScenarioLabel(scenario.Value);
                    string key = string.IsNullOrEmpty(scenario.ID)
                        ? "Scenario|" + scenarioLabel
                        : scenario.ID;
                    string display = string.IsNullOrEmpty(scenario.FriendlyName)
                        ? scenarioLabel
                        : scenario.FriendlyName + " (" + scenarioLabel + ")";

                    int allowedSigners = scenario.ProductSigners?.AllowedSigners?.AllowedSigner?.Length ?? 0;
                    int deniedSigners = scenario.ProductSigners?.DeniedSigners?.DeniedSigner?.Length ?? 0;
                    int fileRulesRefs = scenario.ProductSigners?.FileRulesRef?.FileRuleRef?.Length ?? 0;

                    string description = string.Format(
                        "Scenario={0}; AllowedSigners={1}; DeniedSigners={2}; FileRuleRefs={3}",
                        scenarioLabel, allowedSigners, deniedSigners, fileRulesRefs);

                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, description);
                }
            }
        }

        private static string ScenarioLabel(byte value)
        {
            switch (value)
            {
                case 12: return "User Mode";
                case 131: return "Kernel Mode";
                default: return "Scenario " + value;
            }
        }

        private static void CompareUpdateSigners(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Update Policy Signers";
            foreach (var p in policies)
            {
                if (p.Policy.UpdatePolicySigners == null) continue;
                foreach (var s in p.Policy.UpdatePolicySigners)
                {
                    if (s == null) continue;
                    string key = string.IsNullOrEmpty(s.SignerId) ? "UpdateSigner|" + Guid.NewGuid() : s.SignerId;
                    AddOrUpdateEntry(result, section, key, s.SignerId ?? key, p.DisplayName, "Referenced");
                }
            }
        }

        private static void CompareSupplementalSigners(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Supplemental Policy Signers";
            foreach (var p in policies)
            {
                if (p.Policy.SupplementalPolicySigners == null) continue;
                foreach (var s in p.Policy.SupplementalPolicySigners)
                {
                    if (s == null) continue;
                    string key = string.IsNullOrEmpty(s.SignerId) ? "SuppSigner|" + Guid.NewGuid() : s.SignerId;
                    AddOrUpdateEntry(result, section, key, s.SignerId ?? key, p.DisplayName, "Referenced");
                }
            }
        }

        private static void CompareCiSigners(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "CI Signers";
            foreach (var p in policies)
            {
                if (p.Policy.CiSigners == null) continue;
                foreach (var s in p.Policy.CiSigners)
                {
                    if (s == null) continue;
                    string key = string.IsNullOrEmpty(s.SignerId) ? "CiSigner|" + Guid.NewGuid() : s.SignerId;
                    AddOrUpdateEntry(result, section, key, s.SignerId ?? key, p.DisplayName, "Referenced");
                }
            }
        }

        private static void CompareSettings(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "Settings";
            foreach (var p in policies)
            {
                if (p.Policy.Settings == null) continue;
                foreach (var setting in p.Policy.Settings)
                {
                    if (setting == null) continue;
                    string key = string.Format("{0}|{1}|{2}",
                                               setting.Provider ?? string.Empty,
                                               setting.Key ?? string.Empty,
                                               setting.ValueName ?? string.Empty);
                    string display = string.Format("{0} / {1} / {2}",
                                                   setting.Provider ?? "<none>",
                                                   setting.Key ?? "<none>",
                                                   setting.ValueName ?? "<none>");
                    string value = setting.Value != null && setting.Value.Item != null
                        ? setting.Value.Item.ToString()
                        : "<empty>";
                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, value);
                }
            }
        }

        private static void CompareEKUs(List<LoadedPolicy> policies, ComparisonResult result)
        {
            const string section = "EKUs";
            foreach (var p in policies)
            {
                if (p.Policy.EKUs == null) continue;
                foreach (var eku in p.Policy.EKUs)
                {
                    if (eku == null) continue;
                    string key = string.IsNullOrEmpty(eku.ID) ? "EKU|" + Guid.NewGuid() : eku.ID;
                    string display = string.IsNullOrEmpty(eku.FriendlyName) ? (eku.ID ?? "EKU") : eku.FriendlyName;
                    string value = eku.Value != null && eku.Value.Length > 0
                        ? BitConverter.ToString(eku.Value).Replace("-", string.Empty)
                        : "Present";
                    AddOrUpdateEntry(result, section, key, display, p.DisplayName, value);
                }
            }
        }
    }
}
