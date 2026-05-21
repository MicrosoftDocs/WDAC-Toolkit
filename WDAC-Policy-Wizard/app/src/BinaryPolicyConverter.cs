// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;

namespace WDAC_Wizard
{
    /// <summary>
    /// Converts binary App Control policy files (.cip/.p7b) to XML
    /// using the Microsoft.Security.CodeIntegrity NuGet package.
    /// </summary>
    internal static class BinaryPolicyConverter
    {
        /// <summary>
        /// Converts a binary policy file (.cip/.p7b) to XML using ParseToSiPolicy,
        /// then serializes the resulting SiPolicy object to an XML file on disk.
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

            try
            {
                Logger.Log.AddInfoMsg("BinaryPolicyConverter: Parsing binary policy: " + binaryPolicyPath);
                byte[] binaryData = File.ReadAllBytes(binaryPolicyPath);

                // ParseToSiPolicy returns Microsoft.Security.CodeIntegrity.SiPolicy which is a
                // different type from the Wizard's SiPolicy. Serialize it to XML on disk, then
                // let the Wizard deserialize it with its own type via Helper.DeserializeXMLtoPolicy.
                var ciSiPolicy = Microsoft.Security.CodeIntegrity.Policy.ParseToSiPolicy(binaryData);

                if (ciSiPolicy == null)
                {
                    Logger.Log.AddErrorMsg("BinaryPolicyConverter: ParseToSiPolicy returned null.");
                    return string.Empty;
                }

                // Serialize the CodeIntegrity SiPolicy to XML using its own type's serializer,
                // since it's a different type from the Wizard's SiPolicy
                var serializer = new System.Xml.Serialization.XmlSerializer(ciSiPolicy.GetType());
                using (var writer = new StreamWriter(xmlOutputPath))
                {
                    serializer.Serialize(writer, ciSiPolicy);
                }

                if (File.Exists(xmlOutputPath))
                {
                    Logger.Log.AddInfoMsg("BinaryPolicyConverter: Successfully converted to: " + xmlOutputPath);
                    return xmlOutputPath;
                }

                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Output XML file was not created.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.Log.AddErrorMsg("BinaryPolicyConverter: Conversion failed: " + ex.ToString());
                return string.Empty;
            }
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
    }
}
