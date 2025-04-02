using System;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using System.IO;
using System.Globalization;


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
        /// <param name="filepaths">List of filepaths to try to parse for csv events</param>
        /// <returns>List of CiEvent objects containing policy event info</returns>
        public static List<CiEvent> ReadLogAnalyticCsvFiles(List<string> filepaths)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();

            // Parse each CSV File provided by the user
            foreach (var filepath in filepaths)
            {
                try
                {
                    var records = ReadCsvFile(filepath);

                    // Assert csv must have at least 1 row of data; header is ignored
                    if (records.Count < 1)
                    {
                        throw new Exception(NORECORDS_EXC);
                    }

                    // Assert the following columns must be present
                    if (records[0].Action == null
                       || records[0].AffectedFile== null
                       || records[0].SHA1_Hash == null)
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
        /// Iterates through all of the LogAnalytic records and creates corresponding CiEvent objects
        /// </summary>
        /// <param name="records">Array of LogAnalyticRecord objects to use in generating CiEvents</param>
        /// <returns>List of CiEvent objects containing policy event info</returns>
        public static List<CiEvent> ParseRecordsIntoCiEvents(List<LogAnalyticsRecord> records)
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
        /// <param name="record">Single LogAnalytic CSV record to parse into a policy log event</param>
        /// <param name="eventId">String containing the event ID</param>
        /// <returns>Single CiEvent object containing policy event info</returns>
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
        /// <param name="newEvent">Newly created CI policy event</param>
        /// <param name="existingEvents">List of CI policy events to verify against</param>
        /// <returns>True if event exists in existingEvents. False if unique event.</returns>
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
        /// Replaces common phrases to help with comma removal success
        /// </summary>
        /// <param name="record">Single LogAnalytic CSV record to verify for common issues</param>
        /// <returns>String containing the fixed record</returns>
        private static string ReplaceCommonIssuePhrases(string record)
        {
            /* Common failures include:
             * However,
             * auditing policy,
             * , Inc.
             */
            string[] commonFailures = { "However,", "auditing policy,", ", Inc" };
            string[] replacementValues = { "However", "auditing policy", " Inc" }; 

            for(int i = 0; i < commonFailures.Length; i++)
            {
                if(record.Contains(commonFailures[i]))
                {
                    record = record.Replace(commonFailures[i], replacementValues[i]); 
                }
            }

            return record; 
        }

        /// <summary>
        /// Replaces instances of commas within data arrays to #C#
        /// </summary>
        /// <param name="record">Single LogAnalytic CSV record to verify for common issues</param>
        /// <returns>String containing the fixed record</returns>
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
        /// Reads CSV file provided in filePath and returns a list of LogAnalyticRecords
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static List<LogAnalyticsRecord> ReadCsvFile(string filePath)
        {
            var records = new List<LogAnalyticsRecord>();
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new LogAnalyticsRecord
                    {
                        TimeGenerated = csv.HeaderRecord.Contains("TimeGenerated") ? csv.GetField<string>("TimeGenerated") : null,
                        Computer = csv.HeaderRecord.Contains("Computer") ? csv.GetField<string>("Computer") : null,
                        UserName = csv.HeaderRecord.Contains("UserName") ? csv.GetField<string>("UserName") : null,
                        Action = csv.HeaderRecord.Contains("Action") ? csv.GetField<string>("Action") : null,
                        PublisherValue = csv.HeaderRecord.Contains("publishervalue") ? csv.GetField<string>("publishervalue") : null,
                        PublisherName = csv.HeaderRecord.Contains("PublisherName") ? csv.GetField<string>("PublisherName") : null,
                        Details = csv.HeaderRecord.Contains("Details") ? csv.GetField<string>("Details") : null,
                        AffectedFile = csv.HeaderRecord.Contains("AffectedFile") ? csv.GetField<string>("AffectedFile") : null,
                        ProcessName = csv.HeaderRecord.Contains("ProcessName") ? csv.GetField<string>("ProcessName") : null,
                        Status = csv.HeaderRecord.Contains("Status") ? csv.GetField<string>("Status") : null,
                        PolicyID = csv.HeaderRecord.Contains("PolicyID") ? csv.GetField<string>("PolicyID") : null,
                        PolicyGUID = csv.HeaderRecord.Contains("PolicyGUID") ? csv.GetField<string>("PolicyGUID") : null,
                        PolicyHash = csv.HeaderRecord.Contains("PolicyHash") ? csv.GetField<string>("PolicyHash") : null,
                        SHA1_Hash = csv.HeaderRecord.Contains("SHA1_Hash") ? csv.GetField<string>("SHA1_Hash") : null,
                        OriginalFileName = csv.HeaderRecord.Contains("OriginalFileName") ? csv.GetField<string>("OriginalFileName") : null,
                        InternalName = csv.HeaderRecord.Contains("InternalName") ? csv.GetField<string>("InternalName") : null,
                        FileDescription = csv.HeaderRecord.Contains("FileDescription") ? csv.GetField<string>("FileDescription") : null,
                        ProductName = csv.HeaderRecord.Contains("ProductName") ? csv.GetField<string>("ProductName") : null,
                        FileVersion = csv.HeaderRecord.Contains("FileVersion") ? csv.GetField<string>("FileVersion") : null,
                        PolicyName = csv.HeaderRecord.Contains("PolicyName") ? csv.GetField<string>("PolicyName") : null,
                        SHA256_Hash = csv.HeaderRecord.Contains("SHA256_Hash") ? csv.GetField<string>("SHA256_Hash") : null,
                        IssuerName = csv.HeaderRecord.Contains("IssuerName") ? csv.GetField<string>("IssuerName") : null,
                        NotValidAfter = csv.HeaderRecord.Contains("NotValidAfter") ? csv.GetField<string>("NotValidAfter") : null,
                        NotValidBefore = csv.HeaderRecord.Contains("NotValidBefore") ? csv.GetField<string>("NotValidBefore") : null,
                        PublisherTBSHash = csv.HeaderRecord.Contains("PublisherTBSHash") ? csv.GetField<string>("PublisherTBSHash") : null,
                        IssuerTBSHash = csv.HeaderRecord.Contains("IssuerTBSHash") ? csv.GetField<string>("IssuerTBSHash") : null
                    };
                    records.Add(record);
                }
            }
            return records;
        }

        /// <summary>
        /// CSV row Record class. Each of the row names map to the variable names below in this order. 
        /// </summary>
        public class LogAnalyticsRecord
        {
            public string TimeGenerated;
            public string Computer;
            public string UserName;
            public string Action;
            public string PublisherValue; // header is "publishervalue"
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
    }
}
