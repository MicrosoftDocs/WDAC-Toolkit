// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;

namespace WDAC_Wizard
{
    /// <summary>
    /// Converts binary App Control policy files (.cip/.p7b) to XML.
    /// Uses a 3-tier fallback strategy:
    ///   1. Download latest Microsoft.Security.CodeIntegrity NuGet package from nuget.org
    ///   2. Use the bundled (compile-time referenced) package
    ///   3. Fall back to PowerShell ConvertTo-CIPolicy cmdlet
    /// </summary>
    internal static class BinaryPolicyConverter
    {
        private const string NUGET_PACKAGE_URL = "https://www.nuget.org/api/v2/package/Microsoft.Security.CodeIntegrity";
        private const string CONVERT_METHOD_NAME = "ConvertToCodeIntegrityPolicy";
        private const string POLICY_CLASS_NAME = "Microsoft.Security.CodeIntegrity.Policy";

        /// <summary>
        /// Converts a binary policy file (.cip/.p7b) to XML.
        /// Tries online NuGet package first, then bundled package, then PowerShell fallback.
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

            // Tier 1: Try downloading and using the latest NuGet package
            Logger.Log.AddInfoMsg("BinaryPolicyConverter: Attempting online NuGet package conversion...");
            string result = TryConvertWithOnlinePackage(binaryPolicyPath, xmlOutputPath);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Online NuGet package conversion succeeded.");
                return result;
            }

            // Tier 2: Try using the bundled (compile-time referenced) package
            Logger.Log.AddInfoMsg("BinaryPolicyConverter: Online failed. Attempting bundled package conversion...");
            result = TryConvertWithBundledPackage(binaryPolicyPath, xmlOutputPath);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Bundled package conversion succeeded.");
                return result;
            }

            // Tier 3: Fall back to PowerShell ConvertTo-CIPolicy cmdlet
            Logger.Log.AddInfoMsg("BinaryPolicyConverter: Bundled failed. Attempting PowerShell fallback...");
            result = TryConvertWithPowerShell(binaryPolicyPath);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: PowerShell conversion succeeded.");
                return result;
            }

            Logger.Log.AddErrorMsg("BinaryPolicyConverter: All conversion methods failed for: " + binaryPolicyPath);
            return string.Empty;
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
        /// Tier 1: Download the latest NuGet package and use reflection to call ConvertToCodeIntegrityPolicy
        /// </summary>
        private static string TryConvertWithOnlinePackage(string binaryPath, string xmlOutputPath)
        {
            string tempDir = null;
            try
            {
                tempDir = Path.Combine(Path.GetTempPath(), "WDACWizard_NuGet_" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDir);

                string nupkgPath = Path.Combine(tempDir, "Microsoft.Security.CodeIntegrity.nupkg");

                // Download the latest package
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

                // Extract the nupkg (it's a ZIP file)
                string extractDir = Path.Combine(tempDir, "extracted");
                ZipFile.ExtractToDirectory(nupkgPath, extractDir);

                // Find the DLL for our target framework (net8.0)
                string dllPath = FindPackageDll(extractDir);
                if (string.IsNullOrEmpty(dllPath))
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Could not find CodeIntegrity DLL in downloaded NuGet package.");
                    return string.Empty;
                }

                // Load the assembly and invoke ConvertToCodeIntegrityPolicy via reflection
                return InvokeConversion(dllPath, binaryPath, xmlOutputPath);
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Online NuGet download/conversion failed: " + ex.Message);
                return string.Empty;
            }
            finally
            {
                // Clean up temp directory
                try
                {
                    if (tempDir != null && Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
                catch (Exception cleanupEx)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Failed to clean up temp dir: " + cleanupEx.Message);
                }
            }
        }

        /// <summary>
        /// Tier 2: Use the bundled (compile-time referenced) Microsoft.Security.CodeIntegrity assembly
        /// </summary>
        private static string TryConvertWithBundledPackage(string binaryPath, string xmlOutputPath)
        {
            try
            {
                // Use reflection to check if ConvertToCodeIntegrityPolicy exists in the bundled assembly,
                // since this API may not be present in all package versions
                var policyType = typeof(Microsoft.Security.CodeIntegrity.Policy);
                var convertMethod = policyType.GetMethod(CONVERT_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);

                if (convertMethod == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Bundled package does not contain " + CONVERT_METHOD_NAME);
                    return string.Empty;
                }

                using (var binaryInput = new FileStream(binaryPath, FileMode.Open, FileAccess.Read))
                using (var xmlOutput = new MemoryStream())
                {
                    convertMethod.Invoke(null, new object[] { binaryInput, xmlOutput });
                    File.WriteAllBytes(xmlOutputPath, xmlOutput.ToArray());
                }

                if (File.Exists(xmlOutputPath))
                {
                    return xmlOutputPath;
                }

                return string.Empty;
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
                string result = PSCmdlets.ConvertBinaryToXml(binaryPath);
                return result;
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
            // Prefer net8.0, then net9.0, then net462 as fallback
            string[] preferredFrameworks = { "net8.0", "net9.0", "net462" };

            foreach (var framework in preferredFrameworks)
            {
                string libPath = Path.Combine(extractDir, "lib", framework, "Microsoft.Security.CodeIntegrity.dll");
                if (File.Exists(libPath))
                {
                    return libPath;
                }
            }

            // Try to find any DLL with the right name
            foreach (var file in Directory.GetFiles(extractDir, "Microsoft.Security.CodeIntegrity.dll", SearchOption.AllDirectories))
            {
                return file;
            }

            return string.Empty;
        }

        /// <summary>
        /// Uses reflection to invoke ConvertToCodeIntegrityPolicy from a dynamically loaded assembly
        /// </summary>
        private static string InvokeConversion(string dllPath, string binaryPath, string xmlOutputPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                var policyType = assembly.GetType(POLICY_CLASS_NAME);

                if (policyType == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Could not find type " + POLICY_CLASS_NAME + " in downloaded assembly.");
                    return string.Empty;
                }

                var convertMethod = policyType.GetMethod(CONVERT_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);
                if (convertMethod == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: Method " + CONVERT_METHOD_NAME + " not found in downloaded assembly.");
                    return string.Empty;
                }

                using (var binaryInput = new FileStream(binaryPath, FileMode.Open, FileAccess.Read))
                using (var xmlOutput = new MemoryStream())
                {
                    convertMethod.Invoke(null, new object[] { binaryInput, xmlOutput });
                    File.WriteAllBytes(xmlOutputPath, xmlOutput.ToArray());
                }

                if (File.Exists(xmlOutputPath))
                {
                    return xmlOutputPath;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Reflection-based conversion failed: " + ex.Message);
                return string.Empty;
            }
        }
    }
}
