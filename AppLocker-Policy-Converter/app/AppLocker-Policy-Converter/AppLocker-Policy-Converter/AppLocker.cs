﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.8.3928.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlRootAttribute("AppLockerPolicy", Namespace="", IsNullable=false)]
public partial class AppLockerPolicy {
    
    private RuleCollectionType[] ruleCollectionField;
    
    private PolicyExtensionsType policyExtensionsField;
    
    private decimal versionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("RuleCollection")]
    public RuleCollectionType[] RuleCollection {
        get {
            return this.ruleCollectionField;
        }
        set {
            this.ruleCollectionField = value;
        }
    }
    
    /// <remarks/>
    public PolicyExtensionsType PolicyExtensions {
        get {
            return this.policyExtensionsField;
        }
        set {
            this.policyExtensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal Version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class RuleCollectionType {
    
    private object[] itemsField;
    
    private RuleCollectionExtensionsType ruleCollectionExtensionsField;
    
    private string typeField;
    
    private EnforcementModeType enforcementModeField;
    
    private bool enforcementModeFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("FileHashRule", typeof(FileHashRuleType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePathRule", typeof(FilePathRuleType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePublisherRule", typeof(FilePublisherRuleType))]
    public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
    
    /// <remarks/>
    public RuleCollectionExtensionsType RuleCollectionExtensions {
        get {
            return this.ruleCollectionExtensionsField;
        }
        set {
            this.ruleCollectionExtensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public EnforcementModeType EnforcementMode {
        get {
            return this.enforcementModeField;
        }
        set {
            this.enforcementModeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool EnforcementModeSpecified {
        get {
            return this.enforcementModeFieldSpecified;
        }
        set {
            this.enforcementModeFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FileHashRuleType {
    
    private FileHashRuleConditionsType conditionsField;
    
    private string idField;
    
    private string nameField;
    
    private string descriptionField;
    
    private string userOrGroupSidField;
    
    private RuleActionType actionField;
    
    /// <remarks/>
    public FileHashRuleConditionsType Conditions {
        get {
            return this.conditionsField;
        }
        set {
            this.conditionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string UserOrGroupSid {
        get {
            return this.userOrGroupSidField;
        }
        set {
            this.userOrGroupSidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public RuleActionType Action {
        get {
            return this.actionField;
        }
        set {
            this.actionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FileHashRuleConditionsType {
    
    private FileHashType[] fileHashConditionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("FileHash", IsNullable=false)]
    public FileHashType[] FileHashCondition {
        get {
            return this.fileHashConditionField;
        }
        set {
            this.fileHashConditionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FileHashType {
    
    private HashType typeField;
    
    private string dataField;
    
    private string sourceFileNameField;
    
    private string sourceFileLengthField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public HashType Type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Data {
        get {
            return this.dataField;
        }
        set {
            this.dataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string SourceFileName {
        get {
            return this.sourceFileNameField;
        }
        set {
            this.sourceFileNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string SourceFileLength {
        get {
            return this.sourceFileLengthField;
        }
        set {
            this.sourceFileLengthField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum HashType {
    
    /// <remarks/>
    SHA256,
    
    /// <remarks/>
    SHA256Flat,
    
    /// <remarks/>
    SHA1,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class PluginPolicyType {
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ExecutionCategoryType {
    
    private PluginPolicyType[] policiesField;
    
    private string idField;
    
    private AttributeEnumType[] appidTypesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Policy", IsNullable=false)]
    public PluginPolicyType[] Policies {
        get {
            return this.policiesField;
        }
        set {
            this.policiesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public AttributeEnumType[] AppidTypes {
        get {
            return this.appidTypesField;
        }
        set {
            this.appidTypesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum AttributeEnumType {
    
    /// <remarks/>
    Hash,
    
    /// <remarks/>
    Path,
    
    /// <remarks/>
    Publisher,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class PluginType {
    
    private ExecutionCategoryType[] executionCategoriesField;
    
    private string nameField;
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ExecutionCategory", IsNullable=false)]
    public ExecutionCategoryType[] ExecutionCategories {
        get {
            return this.executionCategoriesField;
        }
        set {
            this.executionCategoriesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ThresholdPolicyExtensionsType {
    
    private PluginType[] pluginsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Plugin", IsNullable=false)]
    public PluginType[] Plugins {
        get {
            return this.pluginsField;
        }
        set {
            this.pluginsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class PolicyExtensionsType {
    
    private ThresholdPolicyExtensionsType thresholdExtensionsField;
    
    private System.Xml.XmlElement[] anyField;
    
    /// <remarks/>
    public ThresholdPolicyExtensionsType ThresholdExtensions {
        get {
            return this.thresholdExtensionsField;
        }
        set {
            this.thresholdExtensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyElementAttribute()]
    public System.Xml.XmlElement[] Any {
        get {
            return this.anyField;
        }
        set {
            this.anyField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class OriginDataRevocationType {
    
    private uint currentOriginDataIdField;
    
    private uint trustedOriginDataIdField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint CurrentOriginDataId {
        get {
            return this.currentOriginDataIdField;
        }
        set {
            this.currentOriginDataIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint TrustedOriginDataId {
        get {
            return this.trustedOriginDataIdField;
        }
        set {
            this.trustedOriginDataIdField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class SystemAppsType {
    
    private AllowSystemAppsType allowField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public AllowSystemAppsType Allow {
        get {
            return this.allowField;
        }
        set {
            this.allowField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum AllowSystemAppsType {
    
    /// <remarks/>
    Enabled,
    
    /// <remarks/>
    NotEnabled,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class RedstoneCollectionExtensionsType {
    
    private SystemAppsType systemAppsField;
    
    private OriginDataRevocationType originDataRevocationField;
    
    /// <remarks/>
    public SystemAppsType SystemApps {
        get {
            return this.systemAppsField;
        }
        set {
            this.systemAppsField = value;
        }
    }
    
    /// <remarks/>
    public OriginDataRevocationType OriginDataRevocation {
        get {
            return this.originDataRevocationField;
        }
        set {
            this.originDataRevocationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ServicesType {
    
    private ServicesEnforcementModeType enforcementModeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ServicesEnforcementModeType EnforcementMode {
        get {
            return this.enforcementModeField;
        }
        set {
            this.enforcementModeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum ServicesEnforcementModeType {
    
    /// <remarks/>
    NotConfigured,
    
    /// <remarks/>
    Enabled,
    
    /// <remarks/>
    ServicesOnly,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ThresholdCollectionExtensionsType {
    
    private ServicesType servicesField;
    
    /// <remarks/>
    public ServicesType Services {
        get {
            return this.servicesField;
        }
        set {
            this.servicesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class RuleCollectionExtensionsType {
    
    private ThresholdCollectionExtensionsType thresholdExtensionsField;
    
    private RedstoneCollectionExtensionsType redstoneExtensionsField;
    
    private System.Xml.XmlElement[] anyField;
    
    /// <remarks/>
    public ThresholdCollectionExtensionsType ThresholdExtensions {
        get {
            return this.thresholdExtensionsField;
        }
        set {
            this.thresholdExtensionsField = value;
        }
    }
    
    /// <remarks/>
    public RedstoneCollectionExtensionsType RedstoneExtensions {
        get {
            return this.redstoneExtensionsField;
        }
        set {
            this.redstoneExtensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyElementAttribute()]
    public System.Xml.XmlElement[] Any {
        get {
            return this.anyField;
        }
        set {
            this.anyField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePathRuleExceptionsType {
    
    private object[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("FileHashCondition", typeof(FileHashConditionType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePathCondition", typeof(FilePathConditionType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePublisherCondition", typeof(FilePublisherConditionType))]
    public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FileHashConditionType {
    
    private FileHashType[] fileHashField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("FileHash")]
    public FileHashType[] FileHash {
        get {
            return this.fileHashField;
        }
        set {
            this.fileHashField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePathConditionType {
    
    private string pathField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Path {
        get {
            return this.pathField;
        }
        set {
            this.pathField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePublisherConditionType {
    
    private FileVersionRangeType binaryVersionRangeField;
    
    private string publisherNameField;
    
    private string productNameField;
    
    private string binaryNameField;
    
    /// <remarks/>
    public FileVersionRangeType BinaryVersionRange {
        get {
            return this.binaryVersionRangeField;
        }
        set {
            this.binaryVersionRangeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string PublisherName {
        get {
            return this.publisherNameField;
        }
        set {
            this.publisherNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ProductName {
        get {
            return this.productNameField;
        }
        set {
            this.productNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string BinaryName {
        get {
            return this.binaryNameField;
        }
        set {
            this.binaryNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FileVersionRangeType {
    
    private string lowSectionField;
    
    private string highSectionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string LowSection {
        get {
            return this.lowSectionField;
        }
        set {
            this.lowSectionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string HighSection {
        get {
            return this.highSectionField;
        }
        set {
            this.highSectionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePathRuleConditionsType {
    
    private FilePathConditionType filePathConditionField;
    
    /// <remarks/>
    public FilePathConditionType FilePathCondition {
        get {
            return this.filePathConditionField;
        }
        set {
            this.filePathConditionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePathRuleType {
    
    private FilePathRuleConditionsType conditionsField;
    
    private FilePathRuleExceptionsType exceptionsField;
    
    private string idField;
    
    private string nameField;
    
    private string descriptionField;
    
    private string userOrGroupSidField;
    
    private RuleActionType actionField;
    
    /// <remarks/>
    public FilePathRuleConditionsType Conditions {
        get {
            return this.conditionsField;
        }
        set {
            this.conditionsField = value;
        }
    }
    
    /// <remarks/>
    public FilePathRuleExceptionsType Exceptions {
        get {
            return this.exceptionsField;
        }
        set {
            this.exceptionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string UserOrGroupSid {
        get {
            return this.userOrGroupSidField;
        }
        set {
            this.userOrGroupSidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public RuleActionType Action {
        get {
            return this.actionField;
        }
        set {
            this.actionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum RuleActionType {
    
    /// <remarks/>
    Allow,
    
    /// <remarks/>
    Deny,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePublisherRuleExceptionsType {
    
    private object[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("FileHashCondition", typeof(FileHashConditionType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePathCondition", typeof(FilePathConditionType))]
    [System.Xml.Serialization.XmlElementAttribute("FilePublisherCondition", typeof(FilePublisherConditionType))]
    public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePublisherRuleConditionsType {
    
    private FilePublisherConditionType filePublisherConditionField;
    
    /// <remarks/>
    public FilePublisherConditionType FilePublisherCondition {
        get {
            return this.filePublisherConditionField;
        }
        set {
            this.filePublisherConditionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FilePublisherRuleType {
    
    private FilePublisherRuleConditionsType conditionsField;
    
    private FilePublisherRuleExceptionsType exceptionsField;
    
    private string idField;
    
    private string nameField;
    
    private string descriptionField;
    
    private string userOrGroupSidField;
    
    private RuleActionType actionField;
    
    /// <remarks/>
    public FilePublisherRuleConditionsType Conditions {
        get {
            return this.conditionsField;
        }
        set {
            this.conditionsField = value;
        }
    }
    
    /// <remarks/>
    public FilePublisherRuleExceptionsType Exceptions {
        get {
            return this.exceptionsField;
        }
        set {
            this.exceptionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string UserOrGroupSid {
        get {
            return this.userOrGroupSidField;
        }
        set {
            this.userOrGroupSidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public RuleActionType Action {
        get {
            return this.actionField;
        }
        set {
            this.actionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
[System.SerializableAttribute()]
public enum EnforcementModeType {
    
    /// <remarks/>
    NotConfigured,
    
    /// <remarks/>
    Enabled,
    
    /// <remarks/>
    AuditOnly,
}
