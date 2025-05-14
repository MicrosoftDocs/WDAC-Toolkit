| [docs](..)  / [using](.) / advanced-hunting.md
|:---|

# Creating a policy from MDE Advanced Hunting events 

This document outlines the steps to create a App Control policy from the Microsoft Defender for Endpoint (MDE) Advanced Hunting events exported as a CSV file. 

## 1. Export the MDE AH Events as CSV ##

The App Control Wizard requires the CSV file in a very specific format. The MDE AH query must follow exactly this format:

```kql
DeviceEvents
| where ActionType == 'AppControlCodeIntegrityDriverRevoked'
    or ActionType == 'AppControlCodeIntegrityPolicyAudited'
    or ActionType == 'AppControlCodeIntegrityPolicyBlocked'
    or ActionType == 'AppControlCodeIntegritySigningInformation'
    or ActionType == 'AppControlCIScriptAudited'
    or ActionType == 'AppControlCIScriptBlocked'
// SigningInfo Fields
| extend IssuerName = parsejson(AdditionalFields).IssuerName
| extend IssuerTBSHash = parsejson(AdditionalFields).IssuerTBSHash
| extend PublisherName = parsejson(AdditionalFields).PublisherName
| extend PublisherTBSHash = parsejson(AdditionalFields).PublisherTBSHash
// Audit/Block Fields
| extend AuthenticodeHash = parsejson(AdditionalFields).AuthenticodeHash
| extend PolicyId = parsejson(AdditionalFields).PolicyID
| extend PolicyName = parsejson(AdditionalFields).PolicyName
// PE Header Fields
| extend OriginalFileName = parsejson(AdditionalFields).OriginalFileName
| extend ProductName = parsejson(AdditionalFields).ProductName
| extend InternalName = parsejson(AdditionalFields).InternalName
| extend FileDescription = parsejson(AdditionalFields).FileDescription
| extend FileVersion = parsejson(AdditionalFields).FileVersion
// Correlation Fields
| extend CorrelationId = parsejson(AdditionalFields).EtwActivityId
// Keep only actionable info for the Wizard
| project
    Timestamp,
    DeviceId,
    DeviceName,
    ActionType,
    FileName,
    FolderPath,
    SHA1,
    SHA256,
    IssuerName,
    IssuerTBSHash,
    PublisherName,
    PublisherTBSHash,
    AuthenticodeHash,
    PolicyId,
    PolicyName,
    OriginalFileName,
    ProductName,
    InternalName,
    FileDescription,
    FileVersion,
    CorrelationId
```

The Wizard is expecting **exactly the following data fields**:
- Timestamp
- DeviceId
- DeviceName 
- ActionType
- FileName
- FolderPath
- SHA1
- SHA256
- IssuerName
- IssuerTBSHash
- PublisherName
- PublisherTBSHash
- AuthenticodeHash (Optional)
- PolicyId         (Optional)
- PolicyName       (Optional)
- OriginalFileName (Optional)
- ProductName      (Optional)
- InternalName     (Optional)
- FileDescription  (Optional)
- FileVersion      (Optional)
- CorrelationId    (Optional)

CSV files without the required fields, or with additional fields, may fail to properly parse. 

Select the export button to extract the MDE AH events to a csv file.

## 2. Using the Wizard to extract MDE events 

To extract the MDE events in the Wizard and build policies off the audit and block event data, follow the instructions documented on the [MS Docs page](https://learn.microsoft.com/windows/security/application-security/application-control/app-control-for-business/design/appcontrol-wizard-parsing-event-logs#mde-advanced-hunting-wdac-event-parsing)

## 3. Export the MDE AH Events as CSV from Splunk ##

Below is a version of Kusto query above rewritten as a Splunk search query. In the example below replace Splunk indexes  **windowseventlogInd** and **XmlWindowsEventLog** with ones used for your Splunk deployment. They might vary between various Splunk deployments. Consult with your Splunk admins for the proper index names.

```spl
(index=windowseventlogInd sourcetype="XmlWindowsEventLog:Microsoft-Windows-CodeIntegrity/Operational" EventID IN(3023,8028,8029,3077,3076)) OR (index=WinEventLog sourcetype="XmlWinEventLog:Microsoft-Windows-CodeIntegrity/Operational" EventID=3089 IssuerTBSHash!="")
| where NOT (isnull(PolicyName) AND EventID!=3089)
| eval File_Name=if(EventID==3077 OR EventID==3076,File_Name, FilePath)
| rex field=File_Name "(?P<FileName2>[^\\\]+)$"
| rename File_Name as FolderPath
| rename FileName2 as FileName
| rename ActivityID as CorrelationId
| rename SHA1_Hash as SHA1
| rename SHA256_Hash as SHA256
| eval CorrelationId=replace(CorrelationId, "{", "")
| eval CorrelationId=replace(CorrelationId, "}", "")
| eval DeviceName=lower(Computer)
| eval DeviceId=DeviceName
| eval Timestamp= strftime(_time, "%m/%d/%Y %I:%M:%S %p")
| eval SHA1=lower(SHA1) 
| eval SHA256=lower(SHA256)
| eval IssuerTBSHash=lower(IssuerTBSHash) 
| eval PublisherTBSHash=lower(PublisherTBSHash) 
| eval ActionType = case(EventID==8029, "AppControlCIScriptBlocked", EventID==8028, "AppControlCIScriptAudited", EventID==3077, "AppControlCodeIntegrityPolicyBlocked", EventID==3076, "AppControlCodeIntegrityPolicyAudited", EventID==3023, "AppControlCodeIntegrityDriverRevoked", EventID==3089, "AppControlCodeIntegritySigningInformation")
| table
    Timestamp,
    DeviceId,
    DeviceName,
    ActionType,
    FileName,
    FolderPath,
    SHA1,
    SHA256,
    IssuerName,
    IssuerTBSHash,
    PublisherName,
    PublisherTBSHash,
    AuthenticodeHash,
    PolicyId,
    PolicyName,
    OriginalFileName,
    ProductName,
    InternalName,
    FileDescription,
    FileVersion,
    CorrelationId
```
