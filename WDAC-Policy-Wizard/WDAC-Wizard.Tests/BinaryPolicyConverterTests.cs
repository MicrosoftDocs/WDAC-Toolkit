// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using Xunit;
using WDAC_Wizard;

namespace WDAC_Wizard.Tests
{
    [Collection("BinaryPolicyConverter")]
    public class BinaryPolicyConverterTests : IDisposable
    {
        private readonly string _resDir;
        private readonly string _binaryPolicyPath;
        private readonly string _expectedXmlPath;
        private readonly string _tempDir;
        private readonly SiPolicy _expectedPolicy;
        private readonly SiPolicy _actualPolicy;

        public BinaryPolicyConverterTests()
        {
            _resDir = Path.Combine(AppContext.BaseDirectory, "res");
            _binaryPolicyPath = Path.Combine(_resDir, "{BB80778A-1F72-49B6-ABBB-B75D1A3D1C58}.cip");
            _expectedXmlPath = Path.Combine(_resDir, "WindowsWorks2026-05-22.xml");

            // Initialize Logger
            string tempLogDir = Path.Combine(Path.GetTempPath(), "WDACWizardTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempLogDir);
            Logger.NewLogger(tempLogDir);

            // Convert binary to XML once and deserialize both policies for comparison
            _tempDir = Path.Combine(Path.GetTempPath(), "WDACTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
            string tempBinaryPath = Path.Combine(_tempDir, Path.GetFileName(_binaryPolicyPath));
            File.Copy(_binaryPolicyPath, tempBinaryPath);

            _expectedPolicy = Helper.DeserializeXMLtoPolicy(_expectedXmlPath);

            string resultXmlPath = BinaryPolicyConverter.ConvertToXml(tempBinaryPath);
            _actualPolicy = Helper.DeserializeXMLtoPolicy(resultXmlPath);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        // =============================================
        // Policy Metadata
        // =============================================

        [Fact]
        public void ConvertToXml_ProducesNonNullPolicy()
        {
            Assert.NotNull(_actualPolicy);
        }

        [Fact]
        public void PolicyId_Matches()
        {
            Assert.Equal(_expectedPolicy.PolicyID, _actualPolicy.PolicyID, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void BasePolicyId_Matches()
        {
            Assert.Equal(_expectedPolicy.BasePolicyID, _actualPolicy.BasePolicyID, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void VersionEx_Matches()
        {
            Assert.Equal(_expectedPolicy.VersionEx, _actualPolicy.VersionEx);
        }

        [Fact]
        public void PlatformId_Matches()
        {
            Assert.Equal(_expectedPolicy.PlatformID, _actualPolicy.PlatformID, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void PolicyType_Matches()
        {
            Assert.Equal(_expectedPolicy.PolicyType, _actualPolicy.PolicyType);
        }

        [Fact]
        public void HvciOptions_Matches()
        {
            Assert.Equal(_expectedPolicy.HvciOptions, _actualPolicy.HvciOptions);
        }

        // =============================================
        // Rules (Policy Rule-Options)
        // =============================================

        [Fact]
        public void Rules_CountMatches()
        {
            Assert.NotNull(_actualPolicy.Rules);
            Assert.Equal(_expectedPolicy.Rules.Length, _actualPolicy.Rules.Length);
        }

        [Fact]
        public void Rules_AllOptionsMatch()
        {
            var expectedOptions = _expectedPolicy.Rules.Select(r => r.Item.ToString()).OrderBy(o => o).ToArray();
            var actualOptions = _actualPolicy.Rules.Select(r => r.Item.ToString()).OrderBy(o => o).ToArray();
            Assert.Equal(expectedOptions, actualOptions);
        }

        // =============================================
        // EKUs — compared by value (byte[]) since IDs
        // are generated and may differ after decompilation
        // =============================================

        [Fact]
        public void EKUs_CountMatches()
        {
            Assert.NotNull(_actualPolicy.EKUs);
            Assert.Equal(_expectedPolicy.EKUs.Length, _actualPolicy.EKUs.Length);
        }

        [Fact]
        public void EKUs_AllValuesPresent()
        {
            // Compare EKU byte[] values — IDs may differ after decompilation
            var expectedValues = _expectedPolicy.EKUs
                .Select(e => BitConverter.ToString(e.Value)).OrderBy(v => v).ToArray();
            var actualValues = _actualPolicy.EKUs
                .Select(e => BitConverter.ToString(e.Value)).OrderBy(v => v).ToArray();
            Assert.Equal(expectedValues, actualValues);
        }

        // =============================================
        // FileRules — compared by content since IDs may
        // be regenerated after decompilation
        // =============================================

        [Fact]
        public void FileRules_CountMatches()
        {
            Assert.NotNull(_actualPolicy.FileRules);
            Assert.Equal(_expectedPolicy.FileRules.Length, _actualPolicy.FileRules.Length);
        }

        [Fact]
        public void FileRules_FileAttribCountMatches()
        {
            var expectedAttribs = _expectedPolicy.FileRules.OfType<FileAttrib>().ToArray();
            var actualAttribs = _actualPolicy.FileRules.OfType<FileAttrib>().ToArray();
            Assert.Equal(expectedAttribs.Length, actualAttribs.Length);
        }

        [Fact]
        public void FileRules_FileAttrib_FileNamesMatch()
        {
            var expectedFileNames = _expectedPolicy.FileRules.OfType<FileAttrib>()
                .Select(f => f.FileName).OrderBy(n => n).ToArray();
            var actualFileNames = _actualPolicy.FileRules.OfType<FileAttrib>()
                .Select(f => f.FileName).OrderBy(n => n).ToArray();
            Assert.Equal(expectedFileNames, actualFileNames);
        }

        [Fact]
        public void FileRules_FileAttrib_VersionRangesMatch()
        {
            // For each expected FileAttrib, find a matching actual FileAttrib with the same
            // FileName, MinimumFileVersion, and MaximumFileVersion. Multiple FileAttribs can
            // share the same FileName but differ in version ranges (e.g. per-OS-version rules).
            var actualAttribs = _actualPolicy.FileRules.OfType<FileAttrib>().ToList();

            foreach (var expected in _expectedPolicy.FileRules.OfType<FileAttrib>())
            {
                var match = actualAttribs.FirstOrDefault(f =>
                    f.FileName == expected.FileName
                    && VersionsEquivalent(f.MinimumFileVersion, expected.MinimumFileVersion)
                    && VersionsEquivalent(f.MaximumFileVersion, expected.MaximumFileVersion));

                Assert.True(match != null,
                    $"No matching FileAttrib found for FileName={expected.FileName}, " +
                    $"MinVer={expected.MinimumFileVersion}, MaxVer={expected.MaximumFileVersion}");

                // Remove matched entry to handle duplicates correctly
                actualAttribs.Remove(match);
            }
        }

        [Fact]
        public void FileRules_AllowRulesCountMatches()
        {
            var expectedAllows = _expectedPolicy.FileRules.OfType<Allow>().ToArray();
            var actualAllows = _actualPolicy.FileRules.OfType<Allow>().ToArray();
            Assert.Equal(expectedAllows.Length, actualAllows.Length);
        }

        [Fact]
        public void FileRules_DenyRulesCountMatches()
        {
            var expectedDenies = _expectedPolicy.FileRules.OfType<Deny>().ToArray();
            var actualDenies = _actualPolicy.FileRules.OfType<Deny>().ToArray();
            Assert.Equal(expectedDenies.Length, actualDenies.Length);
        }

        // =============================================
        // Signers — compared by content (CertRoot type/value,
        // EKU values, publisher) since IDs and names may differ
        // =============================================

        [Fact]
        public void Signers_CountMatches()
        {
            Assert.NotNull(_actualPolicy.Signers);
            Assert.Equal(_expectedPolicy.Signers.Length, _actualPolicy.Signers.Length);
        }

        [Fact]
        public void Signers_CertRootTypes_AllPresent()
        {
            // Build a fingerprint for each signer: CertRoot.Type + CertRoot.Value hex
            var expectedFingerprints = _expectedPolicy.Signers
                .Select(s => GetSignerFingerprint(s)).OrderBy(f => f).ToArray();
            var actualFingerprints = _actualPolicy.Signers
                .Select(s => GetSignerFingerprint(s)).OrderBy(f => f).ToArray();
            Assert.Equal(expectedFingerprints, actualFingerprints);
        }

        [Fact]
        public void Signers_WithPublisher_CountMatches()
        {
            var expectedWithPublisher = _expectedPolicy.Signers.Where(s => s.CertPublisher != null).ToArray();
            var actualWithPublisher = _actualPolicy.Signers.Where(s => s.CertPublisher != null).ToArray();
            Assert.Equal(expectedWithPublisher.Length, actualWithPublisher.Length);
        }

        [Fact]
        public void Signers_PublisherValues_Match()
        {
            var expectedPublishers = _expectedPolicy.Signers
                .Where(s => s.CertPublisher != null)
                .Select(s => s.CertPublisher.Value).OrderBy(v => v).ToArray();
            var actualPublishers = _actualPolicy.Signers
                .Where(s => s.CertPublisher != null)
                .Select(s => s.CertPublisher.Value).OrderBy(v => v).ToArray();
            Assert.Equal(expectedPublishers, actualPublishers);
        }

        [Fact]
        public void Signers_WithFileAttribRef_CountMatches()
        {
            var expectedWithRef = _expectedPolicy.Signers
                .Where(s => s.FileAttribRef != null && s.FileAttribRef.Length > 0).ToArray();
            var actualWithRef = _actualPolicy.Signers
                .Where(s => s.FileAttribRef != null && s.FileAttribRef.Length > 0).ToArray();
            Assert.Equal(expectedWithRef.Length, actualWithRef.Length);
        }

        [Fact]
        public void Signers_EKUDistribution_Matches()
        {
            // Count how many EKUs each signer has — distributions should match
            var expectedEkuCounts = _expectedPolicy.Signers
                .Select(s => s.CertEKU?.Length ?? 0).OrderBy(c => c).ToArray();
            var actualEkuCounts = _actualPolicy.Signers
                .Select(s => s.CertEKU?.Length ?? 0).OrderBy(c => c).ToArray();
            Assert.Equal(expectedEkuCounts, actualEkuCounts);
        }

        // =============================================
        // Signing Scenarios
        // =============================================

        [Fact]
        public void SigningScenarios_CountMatches()
        {
            Assert.NotNull(_actualPolicy.SigningScenarios);
            Assert.Equal(_expectedPolicy.SigningScenarios.Length, _actualPolicy.SigningScenarios.Length);
        }

        [Fact]
        public void SigningScenarios_ScenarioValuesMatch()
        {
            // Signing scenarios are identified by Value (131=KMCI, 12=UMCI)
            var expectedValues = _expectedPolicy.SigningScenarios.Select(s => s.Value).OrderBy(v => v).ToArray();
            var actualValues = _actualPolicy.SigningScenarios.Select(s => s.Value).OrderBy(v => v).ToArray();
            Assert.Equal(expectedValues, actualValues);
        }

        [Fact]
        public void SigningScenarios_KMCI_AllowedSignerCount()
        {
            var expectedKmci = _expectedPolicy.SigningScenarios.First(s => s.Value == 131);
            var actualKmci = _actualPolicy.SigningScenarios.First(s => s.Value == 131);

            int expectedCount = expectedKmci.ProductSigners?.AllowedSigners?.AllowedSigner?.Length ?? 0;
            int actualCount = actualKmci.ProductSigners?.AllowedSigners?.AllowedSigner?.Length ?? 0;
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void SigningScenarios_UMCI_AllowedSignerCount()
        {
            var expectedUmci = _expectedPolicy.SigningScenarios.First(s => s.Value == 12);
            var actualUmci = _actualPolicy.SigningScenarios.First(s => s.Value == 12);

            int expectedCount = expectedUmci.ProductSigners?.AllowedSigners?.AllowedSigner?.Length ?? 0;
            int actualCount = actualUmci.ProductSigners?.AllowedSigners?.AllowedSigner?.Length ?? 0;
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void SigningScenarios_KMCI_DeniedSignerCountMatches()
        {
            var expectedKmci = _expectedPolicy.SigningScenarios.First(s => s.Value == 131);
            var actualKmci = _actualPolicy.SigningScenarios.First(s => s.Value == 131);

            int expectedCount = expectedKmci.ProductSigners?.DeniedSigners?.DeniedSigner?.Length ?? 0;
            int actualCount = actualKmci.ProductSigners?.DeniedSigners?.DeniedSigner?.Length ?? 0;
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void SigningScenarios_UMCI_DeniedSignerCountMatches()
        {
            var expectedUmci = _expectedPolicy.SigningScenarios.First(s => s.Value == 12);
            var actualUmci = _actualPolicy.SigningScenarios.First(s => s.Value == 12);

            int expectedCount = expectedUmci.ProductSigners?.DeniedSigners?.DeniedSigner?.Length ?? 0;
            int actualCount = actualUmci.ProductSigners?.DeniedSigners?.DeniedSigner?.Length ?? 0;
            Assert.Equal(expectedCount, actualCount);
        }

        // =============================================
        // CiSigners
        // =============================================

        [Fact]
        public void CiSigners_CountMatches()
        {
            Assert.NotNull(_actualPolicy.CiSigners);
            Assert.Equal(_expectedPolicy.CiSigners.Length, _actualPolicy.CiSigners.Length);
        }

        // =============================================
        // Settings (Policy Name and ID)
        // =============================================

        [Fact]
        public void Settings_CountMatches()
        {
            Assert.NotNull(_actualPolicy.Settings);
            Assert.Equal(_expectedPolicy.Settings.Length, _actualPolicy.Settings.Length);
        }

        [Fact]
        public void Settings_PolicyNameMatches()
        {
            var expectedName = _expectedPolicy.Settings.FirstOrDefault(s => s.ValueName == "Name");
            var actualName = _actualPolicy.Settings.FirstOrDefault(s => s.ValueName == "Name");
            Assert.NotNull(expectedName);
            Assert.NotNull(actualName);
            Assert.Equal(expectedName.Value.Item.ToString(), actualName.Value.Item.ToString());
        }

        [Fact]
        public void Settings_PolicyIdSettingMatches()
        {
            var expectedId = _expectedPolicy.Settings.FirstOrDefault(s => s.ValueName == "Id");
            var actualId = _actualPolicy.Settings.FirstOrDefault(s => s.ValueName == "Id");
            Assert.NotNull(expectedId);
            Assert.NotNull(actualId);
            Assert.Equal(expectedId.Value.Item.ToString(), actualId.Value.Item.ToString());
        }

        // =============================================
        // Edge Cases
        // =============================================

        [Fact]
        public void ConvertToXml_InvalidPath_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, BinaryPolicyConverter.ConvertToXml("C:\\nonexistent\\fake.cip"));
        }

        [Fact]
        public void ConvertToXml_NullPath_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, BinaryPolicyConverter.ConvertToXml(null));
        }

        [Fact]
        public void ConvertToXml_EmptyPath_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, BinaryPolicyConverter.ConvertToXml(string.Empty));
        }

        // =============================================
        // Helper.IsBinaryPolicyFile
        // =============================================

        [Fact]
        public void IsBinaryPolicyFile_CipExtension_ReturnsTrue()
        {
            Assert.True(Helper.IsBinaryPolicyFile("policy.cip"));
        }

        [Fact]
        public void IsBinaryPolicyFile_P7bExtension_ReturnsTrue()
        {
            Assert.True(Helper.IsBinaryPolicyFile("SiPolicy.p7b"));
        }

        [Fact]
        public void IsBinaryPolicyFile_XmlExtension_ReturnsFalse()
        {
            Assert.False(Helper.IsBinaryPolicyFile("policy.xml"));
        }

        [Fact]
        public void IsBinaryPolicyFile_NullPath_ReturnsFalse()
        {
            Assert.False(Helper.IsBinaryPolicyFile(null));
        }

        [Fact]
        public void IsBinaryPolicyFile_EmptyPath_ReturnsFalse()
        {
            Assert.False(Helper.IsBinaryPolicyFile(string.Empty));
        }

        // =============================================
        // Helpers
        // =============================================

        /// <summary>
        /// Creates a content-based fingerprint for a signer using its CertRoot and EKU values,
        /// since IDs and names may differ after binary decompilation.
        /// </summary>
        private static string GetSignerFingerprint(Signer signer)
        {
            string certRootType = signer.CertRoot?.Type.ToString() ?? "null";
            string certRootValue = signer.CertRoot?.Value != null
                ? BitConverter.ToString(signer.CertRoot.Value)
                : "null";
            string ekuCount = signer.CertEKU?.Length.ToString() ?? "0";
            string publisher = signer.CertPublisher?.Value ?? "";
            return $"{certRootType}|{certRootValue}|{ekuCount}|{publisher}";
        }

        /// <summary>
        /// Compares two version strings for equivalence. Binary decompilation may encode
        /// version components differently (e.g. 1.1.23.0405 may become 1.1.23.405 after
        /// round-tripping through binary format where components are stored as uint16).
        /// Falls back to parsing as Version objects for numeric equivalence.
        /// </summary>
        private static bool VersionsEquivalent(string v1, string v2)
        {
            if (v1 == v2) return true;
            if (v1 == null || v2 == null) return v1 == v2;

            if (Version.TryParse(v1, out var ver1) && Version.TryParse(v2, out var ver2))
            {
                return ver1.Equals(ver2);
            }

            return string.Equals(v1, v2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
