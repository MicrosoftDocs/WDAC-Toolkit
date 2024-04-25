using System;
using System.Linq;
using System.Collections.Generic;
using FileHelpers;

namespace WDAC_Wizard
{
    internal static class AdvancedHunting
    {
        // Event ID constants
        const string DRIVER_REV_EVENT_NAME = "AppControlCodeIntegrityDriverRevoked"; // 3023
        const string AUDIT_EVENT_NAME = "AppControlCodeIntegrityPolicyAudited"; // 3076
        const string BLOCK_EVENT_NAME = "AppControlCodeIntegrityPolicyBlocked"; // 3077
        const string SIGNING_EVENT_NAME = "AppControlCodeIntegritySigningInformation"; // 3089
        const string SCRIPT_AUDIT_EVENT_NAME = "AppControlCIScriptAudited"; // 8028
        const string SCRIPT_BLOCK_EVENT_NAME = "AppControlCIScriptBlocked"; // 8029

        const int DRIVER_REV_EVENT_ID = 3023;
        const int AUDIT_EVENT_ID = 3076;
        const int BLOCK_EVENT_ID = 3077;
        const int SIGNING_EVENT_ID = 3089;
        const int SCRIPT_AUDIT_EVENT_ID = 8028;
        const int SCRIPT_BLOCK_EVENT_ID = 8029;

        // Delimitted value ',' will be replaced with #C#
        const string DEL_VALUE = "#C#";

        // Errors
        const string NORECORDS_EXC = "No Advanced Hunting Records parsed";
        const string HEADERRECORDS_EXC = @"Advanced Hunting Records are not properly formatted. 
                                         The Wizard could not find critical data columns.";
        const string BAD_SIG_PUBNAME = "Unknown";

        private static string LastError = string.Empty;

        const int TIMESMP_LNG = 22; 

        /// <summary>
        /// Parses the CSV file to CiEvent fields
        /// </summary>
        /// <param name="filepath"></param>
        public static List<CiEvent> ReadAdvancedHuntingCsvFiles(List<string> filepaths)
        {
            List<CiEvent> ciEvents = new List<CiEvent>(); 

            var fileHelperEngine = new FileHelperEngine<AdvancedHunting.Record>();
            fileHelperEngine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue; //Read the file and drop bad records

            // Replace any commas like in Zoom Communications, Inc per bug #273
            fileHelperEngine.BeforeReadRecord += FileHelperEngine_BeforeReadRecord;

            // Parse each CSV File provided by the user
            foreach (var filepath in filepaths)
            {
                try
                {
                    var records = fileHelperEngine.ReadFile(filepath);

                    // Assert csv must have a header and 1 row of data
                    if (records.Length < 2)
                    {
                        throw new Exception(NORECORDS_EXC);
                    }

                    // Assert the following columns must be present
                    if (records[0].ActionType == null
                       || records[0].FileName == null
                       || records[0].FolderPath == null
                       || records[0].SHA1 == null
                       || records[0].IssuerTBSHash == null)
                    {
                        throw new Exception(HEADERRECORDS_EXC);
                    }

                    ciEvents.AddRange(ParseRecordsIntoCiEvents(records));

                }
                catch (Exception e)
                {
                    LastError = e.Message;
                    // continue in the case of mixing AH and LogAnalytic csvs
                }
            }

            return ciEvents;
        }

        /// <summary>
        /// Event handler for bad data like commas and malformed timestamps
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="e"></param>
        private static void FileHelperEngine_BeforeReadRecord(EngineBase engine, FileHelpers.Events.BeforeReadEventArgs<Record> e)
        {
            // Replace the line with the fixed version
            e.RecordLine = ReplaceCommasInRecord(e.RecordLine);

            // Bug reported where MDE AH in Local time, not UTC
            // Replace instances of 'Local' Timestamps with UTC
            // "13/03/2024#C# 3:40:13.988 pm" --> 2024-03-13T06:40:13.9880000Z
            e.RecordLine = ConvertTimestampsToUTC(e.RecordLine);
        }

        /// <summary>
        /// Replaces instances of commas within data arrays to #C#
        /// </summary>
        /// <param name="bad"></param>
        /// <returns></returns>
        private static string ReplaceCommasInRecord(string record)
        {
            var fields = record.Split('"');
            if (fields.Length > 1)
            {
                // String replace on the substrings with quotes; skip first and last
                for(int i = 1; i < fields.Length-1; i++)
                {
                    string sString = fields[i];

                    // Filter out this case - ["DigiCert Trusted G4 Code Signing RSA4096 SHA256, 2021 CA1"
                    // | ",1bd538b5ca353f4949201b01e8d3d34794020a6f3a79628c2cb36e0656dc5aaf," "Zoom Video Communications, Inc."]
                    if (sString.Split(',').Length > 2)
                        continue; 

                    sString = sString.Replace(",", DEL_VALUE);
                    sString = sString.Replace("\"", "");
                    fields[i] = sString; 
                }

                return string.Join("", fields); 
            }
            else
            {
                return record;
            }
        }

        /// <summary>
        /// Converts local timestamps to UTC like "13/03/2024#C# 3:40:13.988 pm" --> 2024-03-13T06:40:13.9880000Z
        /// </summary>
        /// <param name="bad"></param>
        /// <returns></returns>
        private static string ConvertTimestampsToUTC(string record)
        {
            string timestampFormat = "dd/MM/yyyy, h:mm:ss.fff tt";
            var fields = record.Split(',');
            if (fields.Length > 1)
            {
                string localTimeStamp = fields[0]; 
                if(localTimeStamp.Contains(DEL_VALUE)) // Non UTC times will contain a comma
                {
                    // Replace comma to help with DateTime parsing
                    localTimeStamp = localTimeStamp.Replace(DEL_VALUE, ",");

                    // Parse the timestamp
                    try
                    {
                        DateTime localDateTime = DateTime.ParseExact(localTimeStamp, timestampFormat, null);

                        // Convert to UTC
                        string utcDateTime = localDateTime.ToUniversalTime().ToString("o");

                        fields[0] = utcDateTime;
                        return string.Join(",", fields);
                    }
                    catch(Exception exp)
                    {
                        return localTimeStamp; 
                    }
                }

            }
            return record;
        }

        /// <summary>
        /// Iterates through all of the AH records and creates corresponding CiEvent objects
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static List<CiEvent> ParseRecordsIntoCiEvents(Record[] records)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            List<SignerEvent> signerEvents = new List<SignerEvent>();

            // Tmp variables
            CiEvent ciEvent = new CiEvent();
            SignerEvent signerEvent = new SignerEvent();

            foreach (var record in records)
            {
                switch (record.ActionType)
                {
                    case DRIVER_REV_EVENT_NAME:
                        ciEvent = Create3023Event(record);
                        break;

                    case AUDIT_EVENT_NAME:
                        ciEvent = Create3076_3077Event(record, AUDIT_EVENT_ID);
                        break;

                    case BLOCK_EVENT_NAME:
                        ciEvent = Create3076_3077Event(record, BLOCK_EVENT_ID);
                        break;

                    case SCRIPT_AUDIT_EVENT_NAME:
                        ciEvent = Create8028_8029Event(record, SCRIPT_AUDIT_EVENT_ID);
                        break;

                    case SCRIPT_BLOCK_EVENT_NAME:
                        ciEvent = Create8028_8029Event(record, SCRIPT_BLOCK_EVENT_ID);
                        break;

                    case SIGNING_EVENT_NAME:
                        signerEvents.Add(Create3089Event(record));
                        continue;

                    default:
                        continue; 
                }

                // De-duplicate audit and block events
                if (!IsDuplicateEvent(ciEvent, ciEvents)) 
                {
                    ciEvents.Add(ciEvent);
                    ciEvent = new CiEvent(); 
                }
            }

            return CorrelateSigningEvents(ciEvents, signerEvents); 
        }

        /// <summary>
        /// Creates a 3023 CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create3023Event(Record record)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = DRIVER_REV_EVENT_ID;
            ciEvent.PolicyGUID = record.PolicyId;
            ciEvent.PolicyName = record.PolicyName; 
            ciEvent.FileName = record.FileName;
            // MDE AH FolderPath is the dir path without the filename
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath) + "\\" + ciEvent.FileName;
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId;
            ciEvent.PolicyId = record.PolicyId;
            ciEvent.PolicyName = record.PolicyName;

            return ciEvent;
        }

        /// <summary>
        /// Creates a 3076/3077 CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create3076_3077Event(Record record, int eventId)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = eventId;
            ciEvent.FileName = record.FileName;
            // MDE AH FolderPath is the dir path without the filename
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath) + "\\" + ciEvent.FileName;
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId;
            ciEvent.PolicyId = record.PolicyId;
            ciEvent.PolicyName = record.PolicyName; 

            return ciEvent;
        }

        /// <summary>
        /// Creates a 8028/8029 Script CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create8028_8029Event(Record record, int eventId)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = eventId;
            ciEvent.FileName = record.FileName;
            // MDE AH FolderPath is the dir path without the filename
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath) + "\\" + ciEvent.FileName;
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId;
            ciEvent.PolicyId = record.PolicyId;
            ciEvent.PolicyName = record.PolicyName;

            return ciEvent;
        }

        /// <summary>
        /// Creates a 3089 Ci SignerEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static SignerEvent Create3089Event(Record record)
        {
            SignerEvent signerEvent = new SignerEvent();
            signerEvent.EventId = SIGNING_EVENT_ID; 
            signerEvent.IssuerName = record.IssuerName;
            signerEvent.IssuerTBSHash = Helper.ConvertHashStringToByte(record.IssuerTBSHash);
            signerEvent.PublisherName = record.PublisherName;
            signerEvent.DeviceId = record.DeviceId;
            signerEvent.Timestamp = record.Timestamp;

            // Replace Delimitted values, if applicable
            // E.g. Zoom Communications#C# Inc --> Zoom Communications, Inc 
            if (signerEvent.IssuerName.Contains(DEL_VALUE))
            {
                signerEvent.IssuerName = signerEvent.IssuerName.Replace(DEL_VALUE, ",");
            }

            if (signerEvent.PublisherName.Contains(DEL_VALUE))
            {
                signerEvent.PublisherName = signerEvent.PublisherName.Replace(DEL_VALUE, ",");
            }

            return signerEvent;
        }

        /// <summary>
        /// Correlate the Ci and Signer events
        /// </summary>
        /// <param name="ciEvents"></param>
        /// <param name="signerEvents"></param>
        /// <returns></returns>
        private static List<CiEvent> CorrelateSigningEvents(List<CiEvent> ciEvents, List<SignerEvent> signerEvents)
        {
            // AH events currently are missing the correlation ID. Work ongoing to fix this.
            // Correlated signer and ci events appear to share the same timestamp to the 23rd-24th position.
            // Example:
            // Timestamp	DeviceId	DeviceName	ActionType	FileName
            // 2023-01-09T21:49:44.19[89403Z]    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegrityPolicyAudited GoogleUpdate.exe
            // 2023-01-09T21:49:44.19[89495Z]    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegritySigningInformation
            // 2023-01-09T21:49:44.19[89539Z]    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegritySigningInformation

            List<CiEvent> correlatedCiEvents = new List<CiEvent>(); 

            foreach(CiEvent ciEvent in ciEvents)
            {
                // Iterate through all the signingEvents to match on Timestamp[0:23]
                foreach(SignerEvent signerEvent in signerEvents)
                {
                    // If there is a DeviceId and timestamp match - correlated events
                    if(signerEvent.DeviceId == ciEvent.DeviceId
                        && DoTimestampsMatch(signerEvent.Timestamp, ciEvent.Timestamp)
                        && IsValidSigner(signerEvent))
                    {
                        // If first/only signer, set the SignerInfo attribute to signer
                        // otherwise, duplicate ciEvent and append to ciEvents
                        if (ciEvent.SignerInfo.DeviceId == null)
                        {
                            ciEvent.SignerInfo = signerEvent;
                            correlatedCiEvents.Add(ciEvent);
                        }
                        else
                        {
                            CiEvent ciEventCopy = ciEvent.Clone();
                            ciEventCopy.SignerInfo = signerEvent;
                            correlatedCiEvents.Add(ciEventCopy);
                        }
                    }
                }

                // In the case where the file is unsigned
                if (ciEvent.SignerInfo.DeviceId == null)
                {
                    correlatedCiEvents.Add(ciEvent);
                }
            }

            return correlatedCiEvents; 
        }

        /// <summary>
        /// Checks that the 2 timestamps are within 10ms of each other indicating the events are likely correlated
        /// </summary>
        /// <param name="timestampA"></param>
        /// <param name="timestampB"></param>
        /// <returns></returns>
        private static bool DoTimestampsMatch(string timestampA, string timestampB)
        {
            return string.CompareOrdinal(timestampA.Substring(0, TIMESMP_LNG),
                                  timestampB.Substring(0, TIMESMP_LNG)) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool IsValidSigner(SignerEvent signerEvent)
        {
            if (signerEvent.PublisherName == BAD_SIG_PUBNAME
                && signerEvent.IssuerName == BAD_SIG_PUBNAME)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the LastError string from the AdvancedHunting class
        /// </summary>
        /// <returns></returns>
        public static string GetLastError()
        {
            return LastError; 
        }

        /// <summary>
        /// Determines whether the new ciEvent created is unique or a duplicate within the event logs
        /// </summary>
        /// <param name="newEvent"></param>
        /// <param name="existingEvents"></param>
        /// <returns></returns>
        private static bool IsDuplicateEvent(CiEvent newEvent, List<CiEvent> existingEvents)
        {
            if (existingEvents.Count == 0)
            {
                return false;
            }

            // Check file identifiers only. No signature event at this point
            byte[] fileHash = newEvent.SHA2 != null ? newEvent.SHA2 : new byte[] { 0 };
            string filePath = newEvent.FilePath;
            int eventId = newEvent.EventId;

            // Check policy hash - could be blocked by different policies in the evtx
            string policyId= newEvent.PolicyId;

            foreach (CiEvent existingEvent in existingEvents)
            {
                byte[] existingEventSHA2 = existingEvent.SHA2 != null ? existingEvent.SHA2 : new byte[] { 0 };
                if (eventId == existingEvent.EventId
                    && filePath == existingEvent.FilePath
                    && policyId == existingEvent.PolicyId
                    && fileHash.SequenceEqual(existingEventSHA2))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// CSV row Record class. Each of the row names map to the variable names below in this order. 
        /// </summary>
        [DelimitedRecord(",")]
        public class Record
        {
            public string Timestamp;
            public string DeviceId;
            public string DeviceName;
            public string ActionType;
            public string FileName;
            public string FolderPath;
            public string SHA1; // This is the PE SHA1
            public string SHA256; // This is the PE SHA256
            public string IssuerName;
            public string IssuerTBSHash;
            public string PublisherName;
            public string PublisherTBSHash;
            public string AuthenticodeHash; //TODO: Make the AuthenticodeHash column nullable -- string?
            public string PolicyId;
            public string PolicyName;
        }

        // AH KQL Query must follow:
        /*
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
         */
    }
}
