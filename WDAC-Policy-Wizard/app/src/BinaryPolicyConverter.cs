// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace WDAC_Wizard
{
    /// <summary>
    /// Converts binary App Control policy files (.cip/.p7b) to XML.
    /// On application load, attempts to download and cache the latest
    /// Microsoft.Security.CodeIntegrity NuGet package. At conversion time,
    /// uses a 3-tier fallback:
    ///   1. Pre-loaded online assembly (downloaded at startup)
    ///   2. Bundled (compile-time referenced) assembly
    ///   3. PowerShell ConvertTo-CIPolicy cmdlet
    ///
    /// The online/bundled tiers use Policy.ParseToSiPolicy(byte[]) to parse the
    /// binary directly into a SiPolicy object, then serialize it to XML on disk.
    /// </summary>
    internal static class BinaryPolicyConverter
    {
        private const string NUGET_PACKAGE_URL = "https://www.nuget.org/api/v2/package/Microsoft.Security.CodeIntegrity";
        private const string PARSE_METHOD_NAME = "ParseToSiPolicy";
        private const string POLICY_CLASS_NAME = "Microsoft.Security.CodeIntegrity.Policy";

        // Cached reflection method from the online package, loaded at startup
        private static MethodInfo _onlineParseMethod;
        private static bool _onlinePackageLoaded;
        private static readonly object _loadLock = new object();

        // Temp directory holding the extracted online package (kept alive for the app session)
        private static string _onlinePackageTempDir;

        /// <summary>
        /// Initializes the converter by attempting to download and cache the latest
        /// NuGet package in the background. Call once at application startup.
        /// </summary>
        public static void InitializeAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    LoadOnlinePackage();
                }
                catch (Exception ex)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Background package load failed: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Downloads and loads the latest NuGet package, caching the ParseToSiPolicy method.
        /// </summary>
        private static void LoadOnlinePackage()
        {
            lock (_loadLock)
            {
                if (_onlinePackageLoaded)
                {
                    return;
                }

                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Downloading latest NuGet package...");
                string tempDir = Path.Combine(Path.GetTempPath(), "WDACWizard_NuGet_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);

                try
                {
                    string nupkgPath = Path.Combine(tempDir, "Microsoft.Security.CodeIntegrity.nupkg");

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(30);
                        using (var response = httpClient.GetAsync(NUGET_PACKAGE_URL).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            using (var fs = new FileStream(nupkgPath, FileMode.Create, FileAccess.Write))
                            {
                                response.Content.CopyToAsync(fs).Wait();
                            }
                        }
                    }

                    string extractDir = Path.Combine(tempDir, "extracted");
                    ZipFile.ExtractToDirectory(nupkgPath, extractDir);

                    string dllPath = FindPackageDll(extractDir);
                    if (string.IsNullOrEmpty(dllPath))
                    {
                        Logger.Log.AddErrorMsg("BinaryPolicyConverter: Could not find CodeIntegrity DLL in downloaded package.");
                        CleanupTempDir(tempDir);
                        return;
                    }

                    var assembly = Assembly.LoadFrom(dllPath);
                    var policyType = assembly.GetType(POLICY_CLASS_NAME);
                    if (policyType == null)
                    {
                        Logger.Log.AddErrorMsg("BinaryPolicyConverter: Type " + POLICY_CLASS_NAME + " not found in downloaded assembly.");
                        CleanupTempDir(tempDir);
                        return;
                    }

                    var method = policyType.GetMethod(PARSE_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);
                    if (method == null)
                    {
                        Logger.Log.AddErrorMsg("BinaryPolicyConverter: Method " + PARSE_METHOD_NAME + " not found in downloaded assembly.");
                        CleanupTempDir(tempDir);
                        return;
                    }

                    _onlineParseMethod = method;
                    _onlinePackageTempDir = tempDir;
                    _onlinePackageLoaded = true;
                    Logger.Log.AddInfoMsg("BinaryPolicyConverter: Online package loaded and cached successfully.");
                }
                catch (Exception ex)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Failed to load online package: " + ex.Message);
                    CleanupTempDir(tempDir);
                }
            }
        }

        /// <summary>
        /// Converts a binary policy file (.cip/.p7b) to XML.
        /// Uses pre-loaded online assembly, then bundled assembly, then PowerShell fallback.
        /// </summary>
        /// <param name="binaryPolicyPath">Path to the .cip or .p7b file</param>
        /// <returns>Path to the converted XML file, or empty string on failure</returns>
        public static string ConvertToXml(string binaryPolicyPath)
        {
            if (string.IsNullOrEmpty(binaryPolicyPath) || !File.Exists(binaryPolicyPath))
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Invalid or missing binary policy path: " + binaryPolicyPath);
                return string.Empty;
            }

            string xmlOutputPath = GetOutputXmlPath(binaryPolicyPath);
            byte[] binaryData = File.ReadAllBytes(binaryPolicyPath);

            // Tier 1: Use the pre-loaded online assembly (downloaded at startup)
            if (_onlinePackageLoaded && _onlineParseMethod != null)
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Using pre-loaded online package...");
                string result = TryParseAndSerialize(_onlineParseMethod, binaryData, xmlOutputPath);
                if (!string.IsNullOrEmpty(result))
                {
                    Logger.Log.AddInfoMsg("BinaryPolicyConverter: Online package conversion succeeded.");
                    return result;
                }
            }
            else
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Online package not available.");
            }

            // Tier 2: Try using the bundled (compile-time referenced) package
            Logger.Log.AddInfoMsg("BinaryPolicyConverter: Attempting bundled package conversion...");
            string bundledResult = TryConvertWithBundledPackage(binaryData, xmlOutputPath);
            if (!string.IsNullOrEmpty(bundledResult))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Bundled package conversion succeeded.");
                return bundledResult;
            }

            // Tier 3: Fall back to PowerShell ConvertTo-CIPolicy cmdlet
            Logger.Log.AddInfoMsg("BinaryPolicyConverter: Attempting PowerShell fallback...");
            string psResult = TryConvertWithPowerShell(binaryPolicyPath);
            if (!string.IsNullOrEmpty(psResult))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: PowerShell conversion succeeded.");
                return psResult;
            }

            Logger.Log.AddErrorMsg("BinaryPolicyConverter: All conversion methods failed for: " + binaryPolicyPath);
            return string.Empty;
        }

        /// <summary>
        /// Cleans up the cached online package temp directory. Call on application exit.
        /// </summary>
        public static void Cleanup()
        {
            CleanupTempDir(_onlinePackageTempDir);
            _onlinePackageTempDir = null;
            _onlineParseMethod = null;
            _onlinePackageLoaded = false;
        }

        /// <summary>
        /// Generates the output XML path for a binary policy file.
        /// Falls back to the user's Documents folder if the source directory
        /// is under System32 (which is typically not writable by standard users).
        /// </summary>
        private static string GetOutputXmlPath(string binaryPolicyPath)
        {
            string dir = Path.GetDirectoryName(binaryPolicyPath);
            string baseName = Path.GetFileNameWithoutExtension(binaryPolicyPath);

            string system32Dir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            if (!string.IsNullOrEmpty(dir) && dir.StartsWith(system32Dir, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Source is under System32. Falling back to Documents folder.");
                dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            return Path.Combine(dir, baseName + "_converted.xml");
        }

        /// <summary>
        /// Invokes ParseToSiPolicy via a cached MethodInfo, then serializes the result to XML
        /// </summary>
        private static string TryParseAndSerialize(MethodInfo parseMethod, byte[] binaryData, string xmlOutputPath)
        {
            try
            {
                var siPolicy = (SiPolicy)parseMethod.Invoke(null, new object[] { binaryData });
                if (siPolicy == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: ParseToSiPolicy returned null.");
                    return string.Empty;
                }

                Helper.SerializePolicytoXML(siPolicy, xmlOutputPath);

                if (File.Exists(xmlOutputPath))
                {
                    return xmlOutputPath;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: ParseToSiPolicy conversion failed: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Tier 2: Use the bundled (compile-time referenced) Microsoft.Security.CodeIntegrity assembly
        /// </summary>
        private static string TryConvertWithBundledPackage(byte[] binaryData, string xmlOutputPath)
        {
            try
            {
                var policyType = typeof(Microsoft.Security.CodeIntegrity.Policy);
                var parseMethod = policyType.GetMethod(PARSE_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);

                if (parseMethod == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Bundled package does not contain " + PARSE_METHOD_NAME);
                    return string.Empty;
                }

                return TryParseAndSerialize(parseMethod, binaryData, xmlOutputPath);
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Bundled package conversion failed: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Tier 3: Fall back to PowerShell ConvertTo-CIPolicy cmdlet
        /// </summary>
        private static string TryConvertWithPowerShell(string binaryPath)
        {
            try
            {
                return PSCmdlets.ConvertBinaryToXml(binaryPath);
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: PowerShell fallback failed: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Searches the extracted NuGet package for the CodeIntegrity DLL,
        /// preferring net8.0 framework target
        /// </summary>
        private static string FindPackageDll(string extractDir)
        {
            string[] preferredFrameworks = { "net8.0", "net9.0", "net462" };

            foreach (var framework in preferredFrameworks)
            {
                string libPath = Path.Combine(extractDir, "lib", framework, "Microsoft.Security.CodeIntegrity.dll");
                if (File.Exists(libPath))
                {
                    return libPath;
                }
            }

            foreach (var file in Directory.GetFiles(extractDir, "Microsoft.Security.CodeIntegrity.dll", SearchOption.AllDirectories))
            {
                return file;
            }

            return string.Empty;
        }

        /// <summary>
        /// Safely deletes a temporary directory
        /// </summary>
        private static void CleanupTempDir(string tempDir)
        {
            try
            {
                if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Failed to clean up temp dir: " + ex.Message);
            }
        }
    }
}
