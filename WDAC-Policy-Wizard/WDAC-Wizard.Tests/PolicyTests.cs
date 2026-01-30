// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Xunit;
using WDAC_Wizard;

namespace WDAC_Wizard.Tests
{
    public class PolicyTests
    {
        #region Helper Methods

        private WDAC_Policy CreateTestPolicy(string version = "10.0.0.0")
        {
            var policy = new WDAC_Policy();
            policy.siPolicy = new SiPolicy
            {
                VersionEx = version
            };
            return policy;
        }

        #endregion

        #region UpdateVersion Tests

        [Fact]
        public void UpdateVersion_StandardVersion_IncrementsLastPart()
        {
            // Arrange
            var policy = CreateTestPolicy("10.0.0.5");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("10.0.0.6", result);
            Assert.Equal("10.0.0.6", policy.VersionNumber);
        }

        [Fact]
        public void UpdateVersion_MaxLastPart_RollsOverCascades()
        {
            // Arrange - when last part is at max (65535):
            // Loop at i=3: 65535 >= MaxValue, so set to 0 and increment position 2 to 1
            // Loop at i=2: position 2 is 1 (not at max), so increment again to 2 and break
            // Result: position gets incremented TWICE (once in rollover, once in loop)
            var policy = CreateTestPolicy("10.0.0.65535");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("10.0.1.0", result);
        }

        [Fact]
        public void UpdateVersion_MaxThirdPart_RollsOverCascades()
        {
            // Arrange - cascading increments happen:
            // i=3: 65535 at position 3, roll to 0, increment position 2 to 65536
            // i=2: 65536 at position 2 (> MaxValue), roll to 0, increment position 1 to 1
            // i=1: position 1 is 1, increment to 2 and break
            var policy = CreateTestPolicy("10.0.65535.65535");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("10.1.0.0", result);
        }

        [Fact]
        public void UpdateVersion_MaxSecondPart_RollsOverCascades()
        {
            // Arrange - cascading rollover to first part
            var policy = CreateTestPolicy("10.65535.65535.65535");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("11.0.0.0", result);
        }

        [Fact]
        public void UpdateVersion_AllPartsAtMax_RollsToZero()
        {
            // Arrange
            var policy = CreateTestPolicy("65535.65535.65535.65535");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("0.0.0.0", result);
        }

        [Fact]
        public void UpdateVersion_ZeroVersion_IncrementsToOne()
        {
            // Arrange
            var policy = CreateTestPolicy("0.0.0.0");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            Assert.Equal("0.0.0.1", result);
        }

        [Fact]
        public void UpdateVersion_MultipleRollovers_HandlesCorrectly()
        {
            // Arrange - cascading rollover behavior
            // Last part at max rolls to 0, increments third to 65536
            // Third part now > max, rolls to 0, increments second to 3
            // But loop continues and third gets incremented again after rollover
            var policy = CreateTestPolicy("1.2.65535.65535");

            // Act
            string result = policy.UpdateVersion();

            // Assert
            // The cascading logic increments position 1 twice due to loop continuation
            Assert.Equal("1.3.0.0", result);
        }

        #endregion

        #region EditPathContainsVersionInfo Tests

        [Fact]
        public void EditPathContainsVersionInfo_ValidPath_ReturnsCorrectIndex()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = "C:\\Policies\\MyPolicy_v10.0.0.1.xml";

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.True(result > 0);
        }

        [Fact]
        public void EditPathContainsVersionInfo_NoVersionInfo_ReturnsZero()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = "C:\\Policies\\MyPolicy.xml";

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void EditPathContainsVersionInfo_NullPath_ReturnsZero()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = null;

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void EditPathContainsVersionInfo_TooShortPath_ReturnsZero()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = "a.xml";

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void EditPathContainsVersionInfo_IncompleteVersion_ReturnsZero()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = "C:\\Policies\\MyPolicy_v10.0.xml";

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void EditPathContainsVersionInfo_MultipleVersionMarkers_UsesLast()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.EditPolicyPath = "C:\\Policies\\v1_MyPolicy_v10.0.0.1.xml";

            // Act
            int result = policy.EditPathContainsVersionInfo();

            // Assert
            Assert.True(result > 0);
        }

        #endregion

        #region HasRuleOption Tests

        [Fact]
        public void HasRuleOption_ExistingOption_ReturnsTrue()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.PolicyRuleOptions = new List<RuleType>
            {
                new RuleType { Item = OptionType.EnabledAuditMode }
            };

            // Act
            bool result = policy.HasRuleOption(OptionType.EnabledAuditMode);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasRuleOption_NonExistingOption_ReturnsFalse()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.PolicyRuleOptions = new List<RuleType>
            {
                new RuleType { Item = OptionType.EnabledAuditMode }
            };

            // Act
            bool result = policy.HasRuleOption(OptionType.EnabledUMCI);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasRuleOption_EmptyList_ReturnsFalse()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.PolicyRuleOptions = new List<RuleType>();

            // Act
            bool result = policy.HasRuleOption(OptionType.EnabledAuditMode);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasRuleOption_MultipleOptions_FindsCorrectOne()
        {
            // Arrange
            var policy = new WDAC_Policy();
            policy.PolicyRuleOptions = new List<RuleType>
            {
                new RuleType { Item = OptionType.EnabledAuditMode },
                new RuleType { Item = OptionType.EnabledUMCI },
                new RuleType { Item = OptionType.EnabledInvalidateEAsonReboot }
            };

            // Act
            bool result = policy.HasRuleOption(OptionType.EnabledUMCI);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region PolicyType Enum Tests

        [Fact]
        public void PolicyType_HasExpectedValues()
        {
            // Assert
            Assert.Equal(0, (int)WDAC_Policy.PolicyType.None);
            Assert.Equal(1, (int)WDAC_Policy.PolicyType.BasePolicy);
            Assert.Equal(2, (int)WDAC_Policy.PolicyType.SupplementalPolicy);
            Assert.Equal(3, (int)WDAC_Policy.PolicyType.AppIdTaggingPolicy);
        }

        #endregion

        #region Format Enum Tests

        [Fact]
        public void Format_HasExpectedValues()
        {
            // Assert
            Assert.Equal(0, (int)WDAC_Policy.Format.None);
            Assert.Equal(1, (int)WDAC_Policy.Format.Legacy);
            Assert.Equal(2, (int)WDAC_Policy.Format.MultiPolicy);
        }

        #endregion
    }

    #region COM Tests

    public class COMTests
    {
        [Fact]
        public void COM_Constructor_InitializesWithNoneProviderType()
        {
            // Act
            var com = new COM();

            // Assert
            Assert.Equal(COM.ProviderType.None, com.Provider);
            Assert.NotNull(com.ValueName);
        }

        [Fact]
        public void COM_ProviderType_HasExpectedValues()
        {
            // Assert
            Assert.Equal(0, (int)COM.ProviderType.None);
            Assert.Equal(1, (int)COM.ProviderType.PowerShell);
            Assert.Equal(2, (int)COM.ProviderType.WSH);
            Assert.Equal(3, (int)COM.ProviderType.IE);
            Assert.Equal(4, (int)COM.ProviderType.VBA);
            Assert.Equal(5, (int)COM.ProviderType.MSI);
            Assert.Equal(6, (int)COM.ProviderType.AllHostIds);
        }

        [Fact]
        public void COM_IsValidRule_AllKeys_ReturnsTrue()
        {
            // Arrange
            var com = new COM();
            com.Guid = "AllKeys";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_ValidGuidWithBraces_ReturnsTrue()
        {
            // Arrange
            var com = new COM();
            com.Guid = "{12345678-1234-1234-1234-123456789012}";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_ValidGuidWithoutBraces_ReturnsTrue()
        {
            // Arrange
            var com = new COM();
            com.Guid = "12345678-1234-1234-1234-123456789012";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_ValidGuidUpperCase_ReturnsTrue()
        {
            // Arrange
            var com = new COM();
            com.Guid = "ABCDEF12-ABCD-ABCD-ABCD-ABCDEF123456";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_ValidGuidMixedCase_ReturnsTrue()
        {
            // Arrange
            var com = new COM();
            com.Guid = "{AbCdEf12-3456-7890-AbCd-EfAbCdEf1234}";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_InvalidGuidFormat_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = "not-a-valid-guid";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_EmptyString_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = "";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_NullGuid_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = null;

            // Act & Assert
            Assert.False(com.IsValidRule());
        }

        [Fact]
        public void COM_IsValidRule_PartialGuid_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = "12345678-1234-1234";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_GuidWithExtraCharacters_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = "{12345678-1234-1234-1234-123456789012}extra";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_GuidWithInvalidCharacters_ReturnsFalse()
        {
            // Arrange
            var com = new COM();
            com.Guid = "GGGGGGGG-1234-1234-1234-123456789012";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_GuidWithoutHyphens_ReturnsTrue()
        {
            // Arrange - Guid.TryParse accepts GUID strings without hyphens
            var com = new COM();
            com.Guid = "12345678123412341234123456789012";

            // Act
            bool result = com.IsValidRule();

            // Assert
            // GUID format without hyphens (N format) is valid
            Assert.True(result);
        }

        [Fact]
        public void COM_IsValidRule_AllKeysCaseSensitive_ReturnsFalse()
        {
            // Arrange - "AllKeys" is case-sensitive
            var com = new COM();
            com.Guid = "allkeys";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_AllKeysUpperCase_ReturnsFalse()
        {
            // Arrange - "AllKeys" is case-sensitive
            var com = new COM();
            com.Guid = "ALLKEYS";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void COM_IsValidRule_RealWorldGuid_ReturnsTrue()
        {
            // Arrange - Using a real COM CLSID for Windows Script Host
            var com = new COM();
            com.Guid = "{72C24DD5-D70A-438B-8A42-98424B88AFB8}";

            // Act
            bool result = com.IsValidRule();

            // Assert
            Assert.True(result);
        }
    }

    #endregion

    #region AppID Tests

    public class AppIDTests
    {
        [Fact]
        public void AppID_IsValidTag_WithValidKeyAndValue_ReturnsTrue()
        {
            // Arrange
            var appId = new AppID
            {
                Key = "TestKey",
                Value = "TestValue"
            };

            // Act
            bool result = appId.IsValidTag();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AppID_IsValidTag_WithEmptyKey_ReturnsFalse()
        {
            // Arrange
            var appId = new AppID
            {
                Key = "",
                Value = "TestValue"
            };

            // Act
            bool result = appId.IsValidTag();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AppID_IsValidTag_WithEmptyValue_ReturnsFalse()
        {
            // Arrange
            var appId = new AppID
            {
                Key = "TestKey",
                Value = ""
            };

            // Act
            bool result = appId.IsValidTag();

            // Assert
            Assert.False(result);
        }
    }

    #endregion

    #region PolicyFileRules Tests

    public class PolicyFileRulesTests
    {
        [Fact]
        public void PolicyFileRules_SetRuleType_HashOnly_SetsHashRule()
        {
            // Arrange
            var fileRule = new PolicyFileRules();
            fileRule.Hash = "ABC123";

            // Act
            fileRule.SetRuleType();

            // Assert
            Assert.Equal(PolicyFileRules.RuleType.Hash, fileRule._RuleType);
        }

        [Fact]
        public void PolicyFileRules_SetRuleType_FileNameOnly_SetsFileNameRule()
        {
            // Arrange
            var fileRule = new PolicyFileRules();
            fileRule.FileName = "notepad.exe";

            // Act
            fileRule.SetRuleType();

            // Assert
            Assert.Equal(PolicyFileRules.RuleType.FileName, fileRule._RuleType);
        }

        [Fact]
        public void PolicyFileRules_SetRuleType_FilePathOnly_SetsFilePathRule()
        {
            // Arrange
            var fileRule = new PolicyFileRules();
            fileRule.FilePath = "C:\\Windows\\System32\\notepad.exe";

            // Act
            fileRule.SetRuleType();

            // Assert
            Assert.Equal(PolicyFileRules.RuleType.FilePath, fileRule._RuleType);
        }

        [Fact]
        public void PolicyFileRules_SetRuleType_FilePathAndFileName_SetsHashRuleDueToLogic()
        {
            // Arrange - when both FilePath and FileName are set, the logic defaults to Hash
            // because the else clause catches any case where Hash is not empty OR
            // both FilePath and FileName are set
            var fileRule = new PolicyFileRules();
            fileRule.FilePath = "C:\\Windows\\System32\\notepad.exe";
            fileRule.FileName = "notepad.exe";

            // Act
            fileRule.SetRuleType();

            // Assert
            // SetRuleType logic: if Hash empty AND FilePath empty -> FileName
            //                    else if Hash empty AND FileName empty -> FilePath
            //                    else -> Hash
            // Since both FilePath and FileName are set, it falls to else clause
            Assert.Equal(PolicyFileRules.RuleType.Hash, fileRule._RuleType);
        }
    }

    #endregion
}
