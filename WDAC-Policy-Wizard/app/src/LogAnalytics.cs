using System;
using System.Linq;
using System.Collections.Generic;
using FileHelpers;
using System.IO; 

namespace WDAC_Wizard
{
    internal static class LogAnalytics
    {
        // Event ID constants
        const string AUDIT_EVENT_ID = "3076";
        const string BLOCK_EVENT_ID = "3077";

        // Delimitted value ',' will be replaced with #C#
        const string DEL_VALUE = "#C#";

        // Errors
        const string NORECORDS_EXC = "No LogAnalytics Records parsed";
        const string HEADERRECORDS_EXC = @"LogAnalytics Records are not properly formatted. 
                                         The Wizard could not find critical data columns.";

        private static string LastError = string.Empty;

        /// <summary>
        /// Parses the CSV file to CiEvent fields
        /// </summary>
        /// <param name="filepath"></param>
        public static List<CiEvent> ReadLogAnalyticCsvFiles(List<string> filepaths)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();

            var fileHelperEngine = new FileHelperEngine<LogAnalytics.LogAnalyticsRecord>();
            fileHelperEngine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue; //Read the file and drop bad records

            // Replace any commas like in Zoom Communications, Inc per bug #273
            fileHelperEngine.BeforeReadRecord += FileHelperEngine_LogAnalyticsBeforeReadRecord;

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
                    if (records[0].Action == null
                       || records[0].AffectedFile== null
                       || records[0].SHA1_Hash == null
                       || records[0].IssuerTBSHash == null)
                    {
                        throw new Exception(HEADERRECORDS_EXC);
                    }

                    ciEvents.AddRange(ParseRecordsIntoCiEvents(records));

                }
                catch (Exception e)
                {
                    LastError = e.Message;
                    // return null; // continue in the case of mixing AH and LogAnalytic csvs
                }
            }

            return ciEvents;
        }


        /// <summary>
        /// Iterates through all of the AH records and creates corresponding CiEvent objects
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public static List<CiEvent> ParseRecordsIntoCiEvents(LogAnalyticsRecord[] records)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            foreach (var record in records)
            {
                switch (record.Action)
                {
                    case AUDIT_EVENT_ID:
                         ciEvent = Create3076_3077Event(record, AUDIT_EVENT_ID);
                         break;
                    
                     case BLOCK_EVENT_ID:
                         ciEvent = Create3076_3077Event(record, BLOCK_EVENT_ID);
                         break;

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

            return ciEvents;
        }

        /// <summary>
        /// Creates a 3076/3077 CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create3076_3077Event(LogAnalyticsRecord record, string eventId)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = Convert.ToInt32(eventId);
            ciEvent.FileName = Path.GetFileName(record.AffectedFile);
            ciEvent.FilePath = Helper.GetDOSPath(record.AffectedFile); // + "\\" + ciEvent.FileName;
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1_Hash);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256_Hash);
            ciEvent.OriginalFilename = record.OriginalFileName;
            ciEvent.InternalFilename = record.InternalName;
            ciEvent.FileDescription = record.FileDescription;
            ciEvent.ProductName = record.ProductName;
            ciEvent.FileVersion = record.FileVersion; 

            // Signing Attributes
            ciEvent.SignerInfo.PublisherName = record.PublisherName;
            ciEvent.SignerInfo.PublisherTBSHash = Helper.ConvertHashStringToByte(record.PublisherTBSHash); 
            ciEvent.SignerInfo.IssuerName = record.IssuerName;
            ciEvent.SignerInfo.IssuerTBSHash = Helper.ConvertHashStringToByte(record.IssuerTBSHash); 

            // Policy
            ciEvent.PolicyId = record.PolicyGUID; 
            ciEvent.PolicyName = record.PolicyName;

            return ciEvent;
        }

        /// <summary>
        /// Determines whether the provided ciEvent unique or a duplicate within the event logs
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

            // Check to see if all of the following match: 
            // 1. FilePath
            // 2. EventId (Audit vs Block)
            // 3. PolicyId (Blocked in different policies --> show each)
            // 4. FileHash (gives version)
            // 5. Publisher TBS (PEhash would be the same across different signatures)
            
            string filePath = newEvent.FilePath;
            int eventId = newEvent.EventId;
            string policyId = newEvent.PolicyId;
            byte[] eventSHA2 = newEvent.SHA2;
            byte[] publisherTBS = newEvent.SignerInfo.PublisherTBSHash != null ? newEvent.SignerInfo.PublisherTBSHash : 
                                    new byte[] { 0 };  // Init to 0 in case file is unsigned

            foreach (CiEvent existingEvent in existingEvents)
            {               
                byte[] existingEventPublisherTBS = existingEvent.SignerInfo.PublisherTBSHash != null ?
                                                    existingEvent.SignerInfo.PublisherTBSHash : new byte[] { 0 };

                if (eventId == existingEvent.EventId
                    && filePath == existingEvent.FilePath
                    && policyId == existingEvent.PolicyId
                    && eventSHA2.SequenceEqual(existingEvent.SHA2)
                    && publisherTBS.SequenceEqual(existingEventPublisherTBS))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event handler for bad data like commas and malformed timestamps
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="e"></param>
        private static void FileHelperEngine_LogAnalyticsBeforeReadRecord(EngineBase engine, 
            FileHelpers.Events.BeforeReadEventArgs<LogAnalyticsRecord> e)
        {
            // Replace the line with the fixed version
            e.RecordLine = ReplaceCommasInRecord(e.RecordLine);
        }

        /// <summary>
        /// Replaces instances of commas within data arrays to #C#
        /// </summary>
        /// <param name="bad"></param>
        /// <returns></returns>
        private static string ReplaceCommasInRecord(string record)
        {
            //string delimiter = "\",";
            //var fields = record.Split(new String[] { delimiter }, StringSplitOptions.None);

            var fields = record.Split('"');
            if (fields.Length > 1)
            {
                // String replace on the substrings with quotes; skip first and last
                for (int i = 1; i < fields.Length - 1; i++)
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
        /// CSV row Record class. Each of the row names map to the variable names below in this order. 
        /// </summary>
        [DelimitedRecord(",")]
        public class LogAnalyticsRecord
        {
            public string TimeGenerated;
            public string Computer;
            public string UserName;
            public string Action;
            public string publishervalue;
            public string PublisherName;
            public string Details;
            public string AffectedFile;
            public string ProcessName;
            public string Status;
            public string PolicyID;
            public string PolicyGUID;
            public string PolicyHash;
            public string SHA1_Hash;
            public string OriginalFileName;
            public string InternalName;
            public string FileDescription;
            public string ProductName;
            public string FileVersion;
            public string PolicyName;
            public string SHA256_Hash;
            public string IssuerName;
            public string NotValidAfter;
            public string NotValidBefore;
            public string PublisherTBSHash;
            public string IssuerTBSHash;
        }
        

        // 30d CSV

        /*
        [DelimitedRecord(",")]
        public class LogAnalyticsRecord
        {
            public string TimeGenerated;
            public string Computer;
            public string UserName;
            public string Action;
            public string publishervalue;
            public string PublisherName;
            public string Details;
            public string AffectedFile;
            public string ProcessName;
            public string Status;
            public string PolicyID;
            public string PolicyGUID;
            public string PolicyHash;
            public string SHA1_Hash;
            public string OriginalFileName;
            public string InternalName;
            public string FileDescription;
            public string ProductName;
            public string FileVersion;
            public string PolicyName;
            public string SHA256_Hash;
            public string IssuerName;
            public string NotValidAfter;
            public string NotValidBefore;
            public string PublisherTBSHash;
            public string IssuerTBSHash;
        }
        */
    }
}
