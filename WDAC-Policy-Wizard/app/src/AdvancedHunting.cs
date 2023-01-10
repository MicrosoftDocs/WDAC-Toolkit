using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        const int DRIVER_REV_EVENT_ID = 3023;
        const int AUDIT_EVENT_ID = 3076;
        const int BLOCK_EVENT_ID = 3077;
        const int SIGNING_EVENT_ID = 3089; 

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
        public static List<CiEvent> ParseCSV(string filepath)
        {
            var fileHelperEngine = new FileHelperEngine<AdvancedHunting.Record>();
            fileHelperEngine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue; //Read the file and drop bad records

            try
            {
                var records = fileHelperEngine.ReadFile(filepath);

                // Assert csv must have a header and 1 row of data
                if(records.Length < 2)
                {
                    throw new Exception(NORECORDS_EXC);
                }

                // Assert the following columns must be present
                if(records[0].ActionType == null
                   || records[0].FileName == null
                   || records[0].FolderPath == null)
                   // || records[0].AdditionalFields == null)
                {
                    throw new Exception(HEADERRECORDS_EXC);
                }

                return ParseRecordsIntoCiEvents(records);

            }
            catch (Exception e)
            {
                LastError = e.Message;
                return null;
            }
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

            foreach (var record in records)
            {
                if(record.ActionType == DRIVER_REV_EVENT_NAME)
                {
                    ciEvents.Add(Create3023Event(record));
                }
                else if (record.ActionType == AUDIT_EVENT_NAME)
                {
                    ciEvents.Add(Create3076Event(record));
                }
                else if (record.ActionType == BLOCK_EVENT_NAME)
                {
                    ciEvents.Add(Create3077Event(record));
                }
                else if(record.ActionType == SIGNING_EVENT_NAME)
                {
                    signerEvents.Add(Create3089Event(record));
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
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath);
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId;

            return ciEvent;
        }

        /// <summary>
        /// Creates a 3076 CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create3076Event(Record record)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = AUDIT_EVENT_ID;
            ciEvent.FileName = record.FileName;
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath);
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId;

            return ciEvent;
        }


        /// <summary>
        /// Creates a 3077 CiEvent from the fields in the AH Record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static CiEvent Create3077Event(Record record)
        {
            CiEvent ciEvent = new CiEvent();
            ciEvent.EventId = BLOCK_EVENT_ID;
            ciEvent.FileName = record.FileName;
            ciEvent.FilePath = Helper.GetDOSPath(record.FolderPath);
            ciEvent.SHA1 = Helper.ConvertHashStringToByte(record.SHA1);
            ciEvent.SHA2 = Helper.ConvertHashStringToByte(record.SHA256);
            ciEvent.Timestamp = record.Timestamp;
            ciEvent.DeviceId = record.DeviceId; 

            return ciEvent; 
        }

        /// <summary>
        /// Creates a 3077 CiEvent from the fields in the AH Record
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
            // 2023-01-09T21:49:44.1989403Z    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegrityPolicyAudited GoogleUpdate.exe
            // 2023-01-09T21:49:44.1989495Z    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegritySigningInformation
            // 2023-01-09T21:49:44.1989539Z    6777cf8827f32a6492a16eda60c3e02d80a697d5 liftoffdemo11   AppControlCodeIntegritySigningInformation

            List<CiEvent> correlatedCiEvents = new List<CiEvent>(); 

            foreach(CiEvent ciEvent in ciEvents)
            {
                // Iterate through all the signingEvents to match on Timestamp[0:23]
                foreach(SignerEvent signerEvent in signerEvents)
                {
                    // If there is a DeviceId and timestamp match - correlated events
                    if(signerEvent.DeviceId == ciEvent.DeviceId
                        && IsTimestampMatch(signerEvent.Timestamp, ciEvent.Timestamp)
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
                if (ciEvents.Count == 0)
                {
                    correlatedCiEvents.Add(ciEvent);
                }
            }

            return correlatedCiEvents; 
        }

        private static bool IsTimestampMatch(string timestampA, string timestampB)
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
            public string AuthenticodeHash;
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
        | project Timestamp,DeviceId,DeviceName,ActionType,FileName,FolderPath,SHA1,SHA256,IssuerName,IssuerTBSHash,PublisherName,PublisherTBSHash,AuthenticodeHash, AdditionalFields
         */
    }
}
