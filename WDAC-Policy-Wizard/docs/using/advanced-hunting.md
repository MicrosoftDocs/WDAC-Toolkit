| [docs](..)  / [using](.) / advanced-hunting.md
|:---|

# Creating a policy from MDE Advanced Hunting events 

This document outlines the steps to create a WDAC (CI) policy from the Microsoft Defender for Endpoint (MDE) Advanced Hunting events exported as a CSV file. 

## 1. Export the MDE AH Events as CSV ##

The WDAC Wizard requires the CSV file in a very specific format. The MDE AH query must follow exactly this format:

```kql
DeviceEvents 
| where ActionType startswith 'AppControlCodeIntegrity' 
// SigningInfo Fields
| extend IssuerName = parsejson(AdditionalFields).IssuerName
| extend IssuerTBSHash = parsejson(AdditionalFields).IssuerTBSHash
| extend PublisherName = parsejson(AdditionalFields).PublisherName
| extend PublisherTBSHash = parsejson(AdditionalFields).PublisherTBSHash
// Audit/Block Fields
| extend AuthenticodeHash = parsejson(AdditionalFields).AuthenticodeHash
| extend PolicyId = parsejson(AdditionalFields).PolicyID
| extend PolicyName = parsejson(AdditionalFields).PolicyName
// Keep only actionable info for the Wizard
| project Timestamp,DeviceId,DeviceName,ActionType,FileName,FolderPath,SHA1,SHA256,IssuerName,IssuerTBSHash,PublisherName,PublisherTBSHash,AuthenticodeHash,PolicyId,PolicyName
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
- AuthenticodeHash
- PolicyId
- PolicyName

CSV files without the required fields, or with additional fields, may fail to properly parse. 

Select the export button to extract the MDE AH events to a csv file.

TODO: How do you use this file in the WDAC wizard

