using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WDAC_Wizard
{
    internal static class EventLog
    {
        // WDAC Events
        // Usermode
        const int AUDIT_PE_ID = 3076;
        const int BLOCK_PE_ID = 3077;

        // Kernel mode
        const int BLOCK_SIG_LEVEL_ID = 3033;
        const int AUDIT_KERNEL_ID = 3067;
        const int BLOCK_KERNEL_ID = 3068;

        // Signature Event
        const int SIG_INFO_ID = 3089;
        const int APP_SIG_INFO_ID = 8038;
        const string BAD_SIG_PUBNAME = "Unknown"; 

        // AppLocker
        const int AUDIT_SCRIPT_ID = 8028;
        const int BLOCK_SCRIPT_ID = 8029;
        // const int COM_OBJ_ID = 8036; 

        /// <summary>
        /// Returns the events logs from the on disk .evtx file paths into CiEvent objects
        /// </summary>
        /// <param name="logPaths">List of .evtx file paths to parse</param>
        /// <returns>List of CiEvent objects</returns>
        public static List<CiEvent> ReadArbitraryEventLogs(List<string> logPaths)
        {
            // Get CiEvents from evtx files on disk
            List<CiEvent> ciEvents = ParseEventLogs(logPaths, PathType.FilePath); 
            return ciEvents;
        }

        /// <summary>
        /// Returns the events logs from the system event logs into CiEvent objects
        /// </summary>
        /// <param ></param>
        /// <returns>List of CiEvent objects</returns>
        public static List<CiEvent> ReadSystemEventLogs()
        {
            List<string> logPaths = new List<string>();
            logPaths.Add(Properties.Resources.EventLogCodeIntegrity);
            logPaths.Add(Properties.Resources.EventLogAppLocker);

            // Get CiEvents from system logs
            List<CiEvent> ciEvents = ParseEventLogs(logPaths, PathType.LogName); 
            return ciEvents;
        }

        /// <summary>
        /// Parses the Code Integrity and AppLocker MSI and Script events into CiEvent objects
        /// </summary>
        /// <param name="logPaths">List of event log names to retrieve events from, or list of paths to the event log
        //     file to retrieve events from.</param>
        /// <param name="pathType">Specifies whether the string used in the path parameter specifies the name of
        //     an event log, or the path to an event log file.</param>
        /// <returns>List of CiEvent objects</returns>
        public static List<CiEvent> ParseEventLogs(List<string> logPaths, PathType pathType)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            List<SignerEvent> ciSignerEvents = new List<SignerEvent>();
            List<SignerEvent> appSignerEvents = new List<SignerEvent>();

            foreach (var logPath in logPaths)
            {
                EventLogReader log = new EventLogReader(logPath, pathType);

                // Read Signature Events first to prepopulate for correlation with audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    // Convert entry to json 
                    // Issue # 382 - hashes converted to strings by Event Forwarding
                    // Additionally, noticed EventLogReader drops empty/null entries so can't rely on indexing since 
                    // the size of the entry.Properities field is not fixed
                    var xnode = XElement.Parse(entry.ToXml());
                    string jsonString = JsonConvert.SerializeXNode(xnode);

                    if (entry.Id == SIG_INFO_ID) // CI Signature events
                    {
                        SignerEvent signerEvent = ReadSignatureEvent(jsonString);
                        if (signerEvent != null)
                        {
                            ciSignerEvents.Add(signerEvent);
                        }
                    }
                    else if (entry.Id == APP_SIG_INFO_ID) // AppLocker Signature events
                    {
                        SignerEvent signerEvent = ReadAppLockerSignatureEvent(jsonString);
                        if (signerEvent != null)
                        {
                            appSignerEvents.Add(signerEvent);
                        }
                    }
                }

                log = new EventLogReader(logPath, pathType);

                // Read all other audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    // Convert entry to json 
                    // Issue # 382 - hashes converted to strings by Event Forwarding
                    // Additionally, noticed EventLogReader drops empty/null entries so can't rely on indexing since 
                    // the size of the entry.Properities field is not fixed
                    var xnode = XElement.Parse(entry.ToXml());
                    string jsonString = JsonConvert.SerializeXNode(xnode);

                    // Audit 3076s and block 3077s
                    if (entry.Id == AUDIT_PE_ID 
                        || entry.Id == BLOCK_PE_ID)
                    {
                        List<CiEvent> auditEvents = ReadPEAuditBlockEvent(jsonString, ciSignerEvents);
                        
                        // Handle parsing errors
                        if (auditEvents == null)
                        {
                            continue;
                        }

                        foreach (CiEvent auditEvent in auditEvents)
                        {
                            if (auditEvent != null)
                            {
                                if (!IsDuplicateEvent(auditEvent, ciEvents))
                                {
                                    ciEvents.Add(auditEvent);
                                }
                            }
                        }
                    }

                    // AppLocker MSI and Script channel
                    else if (entry.Id == AUDIT_SCRIPT_ID 
                             || entry.Id == BLOCK_SCRIPT_ID)
                    {
                        List<CiEvent> appEvents = ReadAppLockerEvents(jsonString, appSignerEvents);

                        // Handle parsing errors
                        if (appEvents == null)
                        {
                            continue;
                        }

                        foreach (CiEvent appEvent in appEvents)
                        {
                            if (appEvent != null)
                            {
                                if (!IsDuplicateEvent(appEvent, ciEvents))
                                {
                                    ciEvents.Add(appEvent);
                                }
                            }
                        }
                    }

                    // Block 3033's
                    else if (entry.Id == BLOCK_SIG_LEVEL_ID)
                    {
                        List<CiEvent> blockSLEvents = ReadSLBlockEvent(jsonString, ciSignerEvents);
                        
                        // Handle parsing errors
                        if(blockSLEvents == null)
                        {
                            continue; 
                        }

                        foreach (CiEvent blockSLEvent in blockSLEvents)
                        {
                            if (blockSLEvent != null)
                            {
                                if (!IsDuplicateEvent(blockSLEvent, ciEvents))
                                {
                                    ciEvents.Add(blockSLEvent);
                                }
                            }
                        }
                    }
                }
            }

            return ciEvents; 
        }

        /// <summary>
        /// Parses the EventRecord for a 3076 audit and 3077 block events into a CiEvent object from json string
        /// </summary>
        /// <param name="jsonString">JSON string representation of the event log data</param>
        /// <returns>List of CiEvent objects</returns>
        public static List<CiEvent> ReadPEAuditBlockEvent(string jsonString, List<SignerEvent> ciSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Event Info
            // Version 5
            try
            {
                var objects = JObject.Parse(jsonString);
                JToken eventData = objects["Event"]["EventData"]["Data"];

                ciEvent.EventId = Convert.ToInt32(objects["Event"]["System"]["EventID"]);
                ciEvent.CorrelationId = objects["Event"]["System"]["Correlation"]["@ActivityID"].ToString();

                // File related info
                string ntfilePath = GetValueString(eventData, "File Name"); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);

                // Windows Event Forwarding appears to cast everything as string
                // Issue #382
                ciEvent.SHA1 = Helper.ConvertHashStringToByte(GetValueString(eventData, "SHA1 Hash")); // PE SHA1 Hash 
                ciEvent.SHA2 = Helper.ConvertHashStringToByte(GetValueString(eventData, "SHA256 Hash")); // PE SHA256 Hash

                ciEvent.OriginalFilename = GetValueString(eventData, "OriginalFileName");
                ciEvent.InternalFilename = GetValueString(eventData, "InternalName");
                ciEvent.FileDescription = GetValueString(eventData, "FileDescription");
                ciEvent.ProductName = GetValueString(eventData, "ProductName");
                ciEvent.FileVersion = GetValueString(eventData, "FileVersion");

                // Policy related info
                ciEvent.PolicyGUID = GetValueString(eventData, "PolicyGUID");
                ciEvent.PolicyName = GetValueString(eventData, "PolicyName");
                ciEvent.PolicyId = GetValueString(eventData, "PolicyID");
                ciEvent.PolicyHash = Helper.ConvertHashStringToByte(GetValueString(eventData, "PolicyHash"));

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in ciSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId
                        && IsValidSigner(signer))
                    {
                        // If first/only signer, set the SignerInfo attribute to signer
                        // otherwise, duplicate ciEvent and append to ciEvents
                        if(ciEvent.SignerInfo.CorrelationId == null)
                        {
                            ciEvent.SignerInfo = signer;
                            ciEvents.Add(ciEvent);
                        }
                        else
                        {
                            CiEvent ciEventCopy = ciEvent.Clone();
                            ciEventCopy.SignerInfo = signer;
                            ciEvents.Add(ciEventCopy);
                        }
                    }
                }

                // In the case where the file is unsigned
                if(ciEvents.Count == 0)
                {
                    ciEvents.Add(ciEvent);
                }
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("ReadPEAuditEvent() encountered the following error", e); 
                return null;
            }

            return ciEvents;
        }


        /// <summary>
        /// Parses the EventRecord for a 8028 or 8029 Script/MSI audit/block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadAppLockerEvents(string jsonString, List<SignerEvent> appSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Version 0
            // Event Info
            try
            {
                var objects = JObject.Parse(jsonString);
                JToken eventData = objects["Event"]["EventData"]["Data"];

                ciEvent.EventId = Convert.ToInt32(objects["Event"]["System"]["EventID"]);
                ciEvent.CorrelationId = objects["Event"]["System"]["Correlation"]["@ActivityID"].ToString();

                // File related info
                string ntfilePath = GetValueString(eventData, "File Name"); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);
                ciEvent.SHA1 = Helper.ConvertHashStringToByte(GetValueString(eventData, "SHA1 Hash")); // SHA1 PE Hash
                ciEvent.SHA2 = Helper.ConvertHashStringToByte(GetValueString(eventData, "SHA256 Hash"));  // SHA256 PE Hash

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in appSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId
                        && IsValidSigner(signer))
                    {
                        // If first/only signer, set the SignerInfo attribute to signer
                        // otherwise, duplicate ciEvent and append to ciEvents
                        if (ciEvent.SignerInfo.CorrelationId == null)
                        {
                            ciEvent.SignerInfo = signer;
                            ciEvents.Add(ciEvent);
                        }
                        else
                        {
                            CiEvent ciEventCopy = ciEvent.Clone();
                            ciEventCopy.SignerInfo = signer;
                            ciEvents.Add(ciEventCopy);
                        }
                    }
                }

                // In the case where the file is unsigned
                if (ciEvents.Count == 0)
                {
                    ciEvents.Add(ciEvent);
                }
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("ReadAppLockerEvents() encountered the following error", e);
                return null;
            }

            return ciEvents;
        }


        /// <summary>
        /// Parses the EventRecord for a 3033 signing level block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadSLBlockEvent(string jsonString, List<SignerEvent> ciSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Version 0
            // Event Info
            try
            {
                var objects = JObject.Parse(jsonString);
                JToken eventData = objects["Event"]["EventData"]["Data"];

                ciEvent.EventId = Convert.ToInt32(objects["Event"]["System"]["EventID"]);
                ciEvent.CorrelationId = objects["Event"]["System"]["Correlation"]["@ActivityID"].ToString();

                // File related info
                string ntfilePath = GetValueString(eventData, "FileNameBuffer"); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);

                // Try to match with pre-populated signer events
                // This will be the only thing to allow on to by pass this block event
                foreach (SignerEvent signer in ciSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId
                        && IsValidSigner(signer))
                    {
                        // If first/only signer, set the SignerInfo attribute to signer
                        // otherwise, duplicate ciEvent and append to ciEvents
                        if (ciEvent.SignerInfo.CorrelationId == null)
                        {
                            ciEvent.SignerInfo = signer;
                            ciEvents.Add(ciEvent);
                        }
                        else
                        {
                            CiEvent ciEventCopy = ciEvent.Clone();
                            ciEventCopy.SignerInfo = signer;
                            ciEvents.Add(ciEventCopy);
                        }
                    }
                }

                // In the case where the file is unsigned
                if (ciEvents.Count == 0)
                {
                    ciEvents.Add(ciEvent);
                }

            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("ReadSLBlockEvent() encountered the following error", e);
                return null;
            }

            return ciEvents;
        }

        /// <summary>
        /// Parses the EventRecord for a 3089 signature event into a signer event object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static SignerEvent ReadSignatureEvent(string jsonString)
        {
            // Version 4 

            SignerEvent signerEvent = new SignerEvent();

            // Event Info
            try
            {
                var objects = JObject.Parse(jsonString);
                JToken eventData = objects["Event"]["EventData"]["Data"];

                signerEvent.EventId = Convert.ToInt32(objects["Event"]["System"]["EventID"]);
                signerEvent.CorrelationId = objects["Event"]["System"]["Correlation"]["@ActivityID"].ToString();

                // Signer related info
                signerEvent.PublisherName = GetValueString(eventData, "PublisherName");
                signerEvent.IssuerName = GetValueString(eventData, "IssuerName");
                signerEvent.IssuerTBSHash = Helper.ConvertHashStringToByte(GetValueString(eventData, "IssuerTBSHash"));
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("ReadSignatureEvent() encountered the following error", e);
                return null;
            }

            return signerEvent; 
        }

        /// <summary>
        /// Parses the EventRecord for a 8038 AppLocker signature event into a signer event object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static SignerEvent ReadAppLockerSignatureEvent(string jsonString)
        {
            // Version 0 

            SignerEvent signerEvent = new SignerEvent();

            // Event Info
            try
            {
                var objects = JObject.Parse(jsonString);
                JToken eventData = objects["Event"]["EventData"]["Data"];

                signerEvent.EventId = Convert.ToInt32(objects["Event"]["System"]["EventID"]);
                signerEvent.CorrelationId = objects["Event"]["System"]["Correlation"]["@ActivityID"].ToString();

                // Signer related info
                signerEvent.PublisherName = GetValueString(eventData, "PublisherName");
                signerEvent.IssuerName = GetValueString(eventData, "IssuerName");
                signerEvent.IssuerTBSHash = Helper.ConvertHashStringToByte(GetValueString(eventData, "IssuerTBSHash"));

            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("ReadAppLockerSignatureEvent() encountered the following error", e);
                return null;
            }

            return signerEvent;
        }

        /// <summary>
        /// Determines whether the new ciEvent created is unique or a duplicate within the event logs
        /// </summary>
        /// <param name="newEvent"></param>
        /// <param name="existingEvents"></param>
        /// <returns></returns>
        private static bool IsDuplicateEvent(CiEvent newEvent, List<CiEvent> existingEvents)
        {
            if(existingEvents.Count == 0)
            {
                return false; 
            }

            // Check file identifiers
            byte[] fileHash = newEvent.SHA2 != null ? newEvent.SHA2 : new byte[] { 0 };
            string filePath = newEvent.FilePath; // could be same file but different path
            int eventId = newEvent.EventId;

            string publisher = newEvent.SignerInfo.PublisherName;

            // Check policy hash - could be blocked by different policies in the evtx
            string policyId = newEvent.PolicyId; 

            foreach(CiEvent existingEvent in existingEvents)
            {
                byte[] existingEventSHA2 = existingEvent.SHA2 != null ? existingEvent.SHA2 : new byte[] { 0 };

                if (eventId == existingEvent.EventId
                    && fileHash.SequenceEqual(existingEventSHA2)
                    && filePath == existingEvent.FilePath
                    && policyId == existingEvent.PolicyId
                    && publisher == existingEvent.SignerInfo.PublisherName)
                {
                    return true;
                }
            }

            return false; 
        }

        /// <summary>
        /// Checks whether the SignerEvent is valid and not "Unknown"/TBSHash = {0}
        /// </summary>
        /// <param name="signer"></param>
        /// <returns></returns>
        private static bool IsValidSigner(SignerEvent signer)
        {
            if(signer.PublisherName == BAD_SIG_PUBNAME
                && signer.IssuerName == BAD_SIG_PUBNAME)
            {
                return false; 
            }

            return true; 
        }

        /// <summary>
        /// Gets EventData value given look-up key e.g. OriginalFileName
        /// </summary>
        /// <param name="eventData">JToken object containing the array of EventData key value pairs</param>
        /// <param name="key">String key to perform value look-up</param>
        /// <returns></returns>
        private static string GetValueString(JToken eventData, string key)
        {
            if (eventData == null) 
            { 
                return null;
            }

            foreach(JToken value in eventData)
            {
                if (value["@Name"].ToString() == key)
                {
                    return value["#text"] != null ? value["#text"].ToString() : null ;
                }
            }

            return null;
        }
    }

    public class CiEvent
    {
        /// <summary>
        /// Event related inffo
        /// </summary>
        public int EventId { get; set; }
        public string CorrelationId { get; set; }
        // AH Correlation:
        public string Timestamp { get; set; }
        public string DeviceId { get; set; }

        // File related info
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] SHA1 { get; set; }
        public byte[] SHA2 { get; set; }

        public string OriginalFilename { get; set; }
        public string InternalFilename { get; set; }
        public string FileDescription { get; set; }
        public string ProductName { get; set; }
        public string FileVersion { get; set; }
        public string PackageFamilyName { get; set; }

        // Signer info - 1 signer per CiEvent
        public SignerEvent SignerInfo {get;set;}

        // Policy related info
        public string PolicyName { get; set; }
        public string PolicyGUID { get; set; }
        public string PolicyId { get; set; }
        public byte[] PolicyHash { get; set; }

        public CiEvent()
        {
            // Init as empty for the properties going into the DataViewGrid for sorting
            this.EventId = 0;
            this.FileName = String.Empty;
            this.ProductName = String.Empty;
            this.PolicyName = String.Empty;

            this.SignerInfo = new SignerEvent();
        }

        // Memberwise clone the CiEvent for case of multiple unique signatures
        public CiEvent Clone()
        {
            return (CiEvent)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// Class for the 3089 signature events
    /// </summary>
    public class SignerEvent
    {
        public int EventId { get; set; }
        public string CorrelationId { get; set; }

        // Signers related info
        public string IssuerName { get; set; }
        public byte[] IssuerTBSHash { get; set; }
        public string PublisherName { get; set; }
        public byte[] PublisherTBSHash { get; set; }

        // AH Device Specific
        public string Timestamp { get; set; }
        public string DeviceId { get; set; }

        public SignerEvent()
        {
            this.PublisherName = String.Empty;
        }
    }
}
