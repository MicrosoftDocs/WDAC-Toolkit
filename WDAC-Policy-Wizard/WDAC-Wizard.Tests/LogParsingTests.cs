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

            CiEvent auditEvent = events[0];
            Assert.Equal("contoso-tool.exe", auditEvent.FileName);
            Assert.Equal("%OSDRIVE%\\Program Files\\Contoso\\App\\contoso-tool.exe", auditEvent.FilePath);
            Assert.Equal("Contoso Deployment Tool", auditEvent.FileDescription);
            Assert.Equal("3.4.1.0", auditEvent.FileVersion);
            Assert.Equal("{a1b2c3d4-1111-4a2b-8c3d-1122334455aa}", auditEvent.PolicyId);
            Assert.Equal("579A240282175C8D78CE9CCFA29ACD0212BB43A5AEE769DE74D6CB313345791C", Convert.ToHexString(auditEvent.SHA2));
            Assert.Equal("CN=Contoso Corporation, O=Contoso Corporation, L=Redmond, S=Washington, C=US", auditEvent.SignerInfo.PublisherName);
            Assert.Equal("0F9680DBEA31C65F6025EDCA4383CCFE0EA017FDB61D8DED46AE87AA39571FFF", Convert.ToHexString(auditEvent.SignerInfo.IssuerTBSHash));

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

            CiEvent auditEvent = events[0];
            Assert.Equal("%OSDRIVE%\\Program Files\\Contoso\\App\\contoso-tool.exe", auditEvent.FilePath);
            Assert.Equal("CN=Contoso Corporation, O=Contoso Corporation, L=Redmond, S=Washington, C=US", auditEvent.SignerInfo.PublisherName);
            Assert.Equal("0F9680DBEA31C65F6025EDCA4383CCFE0EA017FDB61D8DED46AE87AA39571FFF", Convert.ToHexString(auditEvent.SignerInfo.IssuerTBSHash));

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

            AssertPolicyMatchesSnapshot(events, "Expected-LogAnalytics-Policy.xml");
        }

        [Fact]
        public void EvtxLogParsing_GeneratesExpectedPolicy()
        {
            List<CiEvent> events = EventLog.ReadArbitraryEventLogs([FixturePath("WDAC-Wizard-Sample-Events.evtx")]);

            AssertPolicyMatchesSnapshot(events, "Expected-Evtx-Policy.xml");
        }

        private static void AssertPolicyMatchesSnapshot(List<CiEvent> events, string expectedFileName)
        {
            SiPolicy policy = Helper.DeserializeXMLtoPolicy(FixturePath("EmptyWDAC.xml"));
            Assert.NotNull(policy);

            ResetRuleCounters();
            foreach (CiEvent ciEvent in events)
            {
                if (ciEvent.SHA1?.Length > 0 && ciEvent.SHA2?.Length > 0)
                {
                    policy = PolicyEventHelper.AddSiPolicyHashRules(ciEvent, policy);
                }
                else if (ciEvent.SignerInfo?.IssuerTBSHash?.Length > 0)
                {
                    policy = PolicyEventHelper.AddSiPolicyPublisherRule(ciEvent, policy, [0, 1, 0, 0, 0]);
                }
            }

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
    }
}
