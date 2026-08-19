// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Xml.Linq;
using Xunit;
using WDAC_Wizard;

namespace WDAC_Wizard.Tests
{
    [Collection("BinaryPolicyConverter")]
    public class LogParsingTests : IDisposable
    {
        private readonly bool _originalUseEnvVars;
        private readonly string _logDirectory;

        public LogParsingTests()
        {
            _originalUseEnvVars = Properties.Settings.Default.useEnvVars;
            Properties.Settings.Default.useEnvVars = true;

            _logDirectory = Path.Combine(Path.GetTempPath(), "WDACWizardTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_logDirectory);
            Logger.NewLogger(_logDirectory);
        }

        public void Dispose()
        {
            Properties.Settings.Default.useEnvVars = _originalUseEnvVars;
            Logger.Log.CloseLogger();
            Directory.Delete(_logDirectory, true);
        }

        [Fact]
        public void ReadLogAnalyticCsvFiles_ParsesSupportedEventsAndFields()
        {
            List<CiEvent> events = LogAnalytics.ReadLogAnalyticCsvFiles([FixturePath("WDAC-Workbook-Export-Sample.csv")]);

            Assert.Equal([3076, 3077, 8028, 8029], events.Select(ciEvent => ciEvent.EventId));
            AssertCsvAuditEvent_0(events[0]);

            CiEvent scriptBlockEvent = events[3];
            Assert.Equal("setup.vbs", scriptBlockEvent.FileName);
            Assert.Equal("%OSDRIVE%\\Users\\Public\\Downloads\\setup.vbs", scriptBlockEvent.FilePath);
            Assert.Equal("B4DD9F612A348A1FBEF28CB61B957EF9DEC7A2BA65F3C3BDDE58A2AF750291B5", Convert.ToHexString(scriptBlockEvent.SHA2));
        }

        [Fact]
        public void ReadArbitraryEventLogs_ParsesAndCorrelatesSignerEvents()
        {
            List<CiEvent> events = EventLog.ReadArbitraryEventLogs([FixturePath("WDAC-Wizard-Sample-Events.evtx")]);

            Assert.Equal([3076, 3077, 3033, 8028, 8029], events.Select(ciEvent => ciEvent.EventId));
            AssertEvtxAuditEvent_0(events[0]);

            CiEvent driverEvent = events[2];
            Assert.Equal("%SYSTEM32%\\drivers\\thirdparty-filter.sys", driverEvent.FilePath);
            Assert.Equal("CN=Fabrikam Drivers, O=Fabrikam Inc, C=US", driverEvent.SignerInfo.PublisherName);
            Assert.Equal("45D7D929D38203FA7E4287CC521756A220CC05901BA289FB2A2B119D30FEE908", Convert.ToHexString(driverEvent.SignerInfo.IssuerTBSHash));

            CiEvent scriptAuditEvent = events[3];
            Assert.Equal("%OSDRIVE%\\Program Files\\Contoso\\Scripts\\deploy.ps1", scriptAuditEvent.FilePath);
            Assert.Equal("CN=Contoso Corporation, O=Contoso Corporation, C=US", scriptAuditEvent.SignerInfo.PublisherName);
            Assert.Equal("4C0E23CCDFF91B55627D7A763255112F777F20E5C5DF117604689867A8C84913", Convert.ToHexString(scriptAuditEvent.SignerInfo.IssuerTBSHash));

            Assert.Empty(events[1].SignerInfo.PublisherName);
            Assert.Empty(events[4].SignerInfo.PublisherName);
        }

        [Fact]
        public void CsvLogParsing_GeneratesExpectedPolicy()
        {
            List<CiEvent> events = LogAnalytics.ReadLogAnalyticCsvFiles([FixturePath("WDAC-Workbook-Export-Sample.csv")]);

            AssertPolicyMatchesSnapshot("Expected-LogAnalytics-Policy.xml", policy =>
            {
                foreach (CiEvent ciEvent in events)
                {
                    policy = PolicyEventHelper.AddSiPolicyHashRules(ciEvent, policy);
                }

                return policy;
            });
        }

        [Fact]
        public void EvtxLogParsing_GeneratesExpectedPolicy()
        {
            List<CiEvent> events = EventLog.ReadArbitraryEventLogs([FixturePath("WDAC-Wizard-Sample-Events.evtx")]);

            AssertPolicyMatchesSnapshot("Expected-Evtx-Policy.xml", policy =>
            {
                foreach (CiEvent ciEvent in events)
                {
                    if (ciEvent.SHA1?.Length > 0 && ciEvent.SHA2?.Length > 0)
                    {
                        policy = PolicyEventHelper.AddSiPolicyHashRules(ciEvent, policy);
                    }
                    else if (ciEvent.SignerInfo?.IssuerTBSHash?.Length > 0)
                    {
                        policy = PolicyEventHelper.AddSiPolicyPublisherRule(ciEvent, policy, [1, 1, 0, 0, 0]);
                    }
                }

                return policy;
            });
        }

        [Fact]
        public void AuditEventHashRule_GeneratesExpectedPolicy()
        {
            CiEvent auditEvent = GetCsvAuditEvent();

            AssertPolicyMatchesSnapshot(
                "Expected-Audit-Hash-Policy.xml",
                policy => PolicyEventHelper.AddSiPolicyHashRules(auditEvent, policy));
        }

        [Fact]
        public void AuditEventPathRules_GenerateExpectedPolicy()
        {
            CiEvent auditEvent = GetCsvAuditEvent();

            AssertPolicyMatchesSnapshot(
                "Expected-Audit-Path-Policy.xml",
                policy => PolicyEventHelper.AddSiPolicyFilePathRule(auditEvent, policy, [1, 1]));
        }

        [Fact]
        public void AuditEventPublisherRule_GeneratesExpectedPolicy()
        {
            CiEvent auditEvent = GetCsvAuditEvent();

            AssertPolicyMatchesSnapshot(
                "Expected-Audit-Publisher-Policy.xml",
                policy => PolicyEventHelper.AddSiPolicyPublisherRule(auditEvent, policy, [1, 1, 0, 0, 0]));
        }

        [Fact]
        public void AuditEventPublisherWithOriginalNameAndVersion_GeneratesExpectedPolicy()
        {
            CiEvent auditEvent = GetCsvAuditEvent();

            AssertPolicyMatchesSnapshot(
                "Expected-Audit-Publisher-OriginalName-Version-Policy.xml",
                policy => PolicyEventHelper.AddSiPolicyPublisherRule(auditEvent, policy, [1, 1, 1, 1, 0]));
        }

        [Fact]
        public void AuditEventAllAvailableFileAttributes_GenerateExpectedPolicy()
        {
            CiEvent auditEvent = GetCsvAuditEvent();

            AssertPolicyMatchesSnapshot(
                "Expected-Audit-All-FileAttributes-Policy.xml",
                policy => PolicyEventHelper.AddSiPolicyFileAttributeRule(auditEvent, policy, [1, 1, 1, 1, 0]));
        }

        [Fact]
        public void ReadLogAnalyticCsvFiles_HeaderOnly_ReturnsNoEventsAndError()
        {
            string csvPath = TempFilePath("header-only.csv");
            File.WriteAllText(csvPath, "Action,AffectedFile,SHA1_Hash");

            List<CiEvent> events = LogAnalytics.ReadLogAnalyticCsvFiles([csvPath]);

            Assert.Empty(events);
            Assert.Equal("No LogAnalytics Records parsed", LogAnalytics.GetLastError());
        }

        [Fact]
        public void ReadLogAnalyticCsvFiles_MissingCriticalHeader_ReturnsNoEventsAndError()
        {
            string csvPath = TempFilePath("missing-header.csv");
            File.WriteAllText(csvPath, "Action,AffectedFile\r\n3076,C:\\Contoso\\app.exe");

            List<CiEvent> events = LogAnalytics.ReadLogAnalyticCsvFiles([csvPath]);

            Assert.Empty(events);
            Assert.Contains("LogAnalytics Records are not properly formatted", LogAnalytics.GetLastError());
        }

        [Fact]
        public void ParseRecordsIntoCiEvents_DuplicateRecords_ReturnsSingleEvent()
        {
            var record = new LogAnalytics.LogAnalyticsRecord
            {
                Action = "3076",
                AffectedFile = @"C:\Contoso\app.exe",
                PolicyGUID = "{11111111-2222-3333-4444-555555555555}",
                SHA1_Hash = "64863b7b0e22bf3d8b5791cebaeccd9e5cd2dc1e",
                SHA256_Hash = "579a240282175c8d78ce9ccfa29acd0212bb43a5aee769de74d6cb313345791c",
                PublisherTBSHash = "3a1f9c77e0b25d84c611aa0398fe4471d2c8b6e5f09a7314bd52ee80cf19a6d2"
            };

            List<CiEvent> events = LogAnalytics.ParseRecordsIntoCiEvents([record, record]);

            Assert.Single(events);
        }

        [Fact]
        public void ReadPEAuditBlockEvent_MalformedJson_ReturnsNull()
        {
            List<CiEvent> events = EventLog.ReadPEAuditBlockEvent("{not-json", []);

            Assert.Null(events);
        }

        private static void AssertPolicyMatchesSnapshot(string expectedFileName, Func<SiPolicy, SiPolicy> buildPolicy)
        {
            SiPolicy policy = Helper.DeserializeXMLtoPolicy(FixturePath("EmptyWDAC.xml"));
            Assert.NotNull(policy);

            ResetRuleCounters();
            policy = buildPolicy(policy);

            string actualPath = Path.Combine(Path.GetTempPath(), expectedFileName);
            Helper.SerializePolicytoXML(policy, actualPath);

            string expectedPath = FixturePath(expectedFileName);
            Assert.True(File.Exists(expectedPath), $"Expected policy snapshot is missing. Computed policy: {actualPath}");

            string expectedXml = NormalizeXml(expectedPath);
            string actualXml = NormalizeXml(actualPath);
            Assert.True(
                string.Equals(expectedXml, actualXml, StringComparison.Ordinal),
                $"Computed policy did not match {expectedFileName}. Computed policy: {actualPath}");
            File.Delete(actualPath);
        }

        private static string NormalizeXml(string path)
        {
            return XDocument.Load(path).ToString(SaveOptions.DisableFormatting);
        }

        private static CiEvent GetCsvAuditEvent()
        {
            List<CiEvent> events = LogAnalytics.ReadLogAnalyticCsvFiles([FixturePath("WDAC-Workbook-Export-Sample.csv")]);
            return Assert.Single(events, ciEvent => ciEvent.EventId == 3076);
        }

        private static void AssertCsvAuditEvent_0(CiEvent auditEvent)
        {
            Assert.Equal(3076, auditEvent.EventId);
            Assert.Null(auditEvent.CorrelationId);
            Assert.Equal("2026-07-06T15:00:14.0000000Z", auditEvent.Timestamp);
            Assert.Equal("AZL-SEC-LAB01", auditEvent.DeviceId);
            Assert.Equal("contoso-tool.exe", auditEvent.FileName);
            Assert.Equal("%OSDRIVE%\\Program Files\\Contoso\\App\\contoso-tool.exe", auditEvent.FilePath);
            Assert.Equal("64863B7B0E22BF3D8B5791CEBAECCD9E5CD2DC1E", Convert.ToHexString(auditEvent.SHA1));
            Assert.Equal("579A240282175C8D78CE9CCFA29ACD0212BB43A5AEE769DE74D6CB313345791C", Convert.ToHexString(auditEvent.SHA2));
            Assert.Equal("contoso-tool.exe", auditEvent.OriginalFilename);
            Assert.Equal("contoso-tool", auditEvent.InternalFilename);
            Assert.Equal("Contoso Deployment Tool", auditEvent.FileDescription);
            Assert.Equal("Contoso Suite", auditEvent.ProductName);
            Assert.Equal("3.4.1.0", auditEvent.FileVersion);
            Assert.Null(auditEvent.PackageFamilyName);
            Assert.Equal("Azure Local Base Audit Policy", auditEvent.PolicyName);
            Assert.Equal("{a1b2c3d4-1111-4a2b-8c3d-1122334455aa}", auditEvent.PolicyGUID);
            Assert.Equal("10000000-2000-3000-4000-500000000001", auditEvent.PolicyId);
            Assert.Equal("0BD740400A2C78451A9912F54215725E212B7FD97D17F9CC90E960639E8CA09F", Convert.ToHexString(auditEvent.PolicyHash));

            Assert.Equal(0, auditEvent.SignerInfo.EventId);
            Assert.Null(auditEvent.SignerInfo.CorrelationId);
            Assert.Equal("CN=Contoso Code Signing CA, O=Contoso Corporation, C=US", auditEvent.SignerInfo.IssuerName);
            Assert.Equal("0F9680DBEA31C65F6025EDCA4383CCFE0EA017FDB61D8DED46AE87AA39571FFF", Convert.ToHexString(auditEvent.SignerInfo.IssuerTBSHash));
            Assert.Empty(auditEvent.SignerInfo.IssuerTBSHashString);
            Assert.Equal("CN=Contoso Corporation, O=Contoso Corporation, L=Redmond, S=Washington, C=US", auditEvent.SignerInfo.PublisherName);
            Assert.Equal("3A1F9C77E0B25D84C611AA0398FE4471D2C8B6E5F09A7314BD52EE80CF19A6D2", Convert.ToHexString(auditEvent.SignerInfo.PublisherTBSHash));
            Assert.Equal(auditEvent.Timestamp, auditEvent.SignerInfo.Timestamp);
            Assert.Equal(auditEvent.DeviceId, auditEvent.SignerInfo.DeviceId);
        }

        private static void AssertEvtxAuditEvent_0(CiEvent auditEvent)
        {
            Assert.Equal(3076, auditEvent.EventId);
            Assert.Equal("{4d6ce5ba-456f-4224-899f-f260f8528de3}", auditEvent.CorrelationId);
            Assert.Null(auditEvent.Timestamp);
            Assert.Null(auditEvent.DeviceId);
            Assert.Equal("contoso-tool.exe", auditEvent.FileName);
            Assert.Equal("%OSDRIVE%\\Program Files\\Contoso\\App\\contoso-tool.exe", auditEvent.FilePath);
            Assert.Equal("64863B7B0E22BF3D8B5791CEBAECCD9E5CD2DC1E", Convert.ToHexString(auditEvent.SHA1));
            Assert.Equal("579A240282175C8D78CE9CCFA29ACD0212BB43A5AEE769DE74D6CB313345791C", Convert.ToHexString(auditEvent.SHA2));
            Assert.Equal("contoso-tool.exe", auditEvent.OriginalFilename);
            Assert.Equal("contoso-tool", auditEvent.InternalFilename);
            Assert.Equal("Contoso Deployment Tool", auditEvent.FileDescription);
            Assert.Equal("Contoso Suite", auditEvent.ProductName);
            Assert.Equal("3.4.1.0", auditEvent.FileVersion);
            Assert.Null(auditEvent.PackageFamilyName);
            Assert.Equal("Azure Local Base Audit Policy", auditEvent.PolicyName);
            Assert.Equal("{a1b2c3d4-1111-4a2b-8c3d-1122334455aa}", auditEvent.PolicyGUID);
            Assert.Equal("10000000-2000-3000-4000-500000000001", auditEvent.PolicyId);
            Assert.Equal("0BD740400A2C78451A9912F54215725E212B7FD97D17F9CC90E960639E8CA09F", Convert.ToHexString(auditEvent.PolicyHash));

            Assert.Equal(3089, auditEvent.SignerInfo.EventId);
            Assert.Equal(auditEvent.CorrelationId, auditEvent.SignerInfo.CorrelationId);
            Assert.Equal("CN=Contoso Code Signing CA, O=Contoso Corporation, C=US", auditEvent.SignerInfo.IssuerName);
            Assert.Equal("0F9680DBEA31C65F6025EDCA4383CCFE0EA017FDB61D8DED46AE87AA39571FFF", Convert.ToHexString(auditEvent.SignerInfo.IssuerTBSHash));
            Assert.Equal("0f9680dbea31c65f6025edca4383ccfe0ea017fdb61d8ded46ae87aa39571fff", auditEvent.SignerInfo.IssuerTBSHashString);
            Assert.Equal("CN=Contoso Corporation, O=Contoso Corporation, L=Redmond, S=Washington, C=US", auditEvent.SignerInfo.PublisherName);
            Assert.Null(auditEvent.SignerInfo.PublisherTBSHash);
            Assert.Null(auditEvent.SignerInfo.Timestamp);
            Assert.Null(auditEvent.SignerInfo.DeviceId);
        }

        private static void ResetRuleCounters()
        {
            PolicyEventHelper.cPublisherRules = 0;
            PolicyEventHelper.cFilePublisherRules = 0;
            PolicyEventHelper.cFileAttribRules = 0;
            PolicyEventHelper.cFilePathRules = 0;
            PolicyEventHelper.cFileHashRules = 0;
        }

        private static string FixturePath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "res", "LogParsing", fileName);
        }

        private string TempFilePath(string fileName)
        {
            return Path.Combine(_logDirectory, fileName);
        }
    }
}
