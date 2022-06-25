using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;


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

        // AppLocker
        const int AUDIT_SCRIPT_ID = 8028;
        const int BLOCK_SCRIPT_ID = 8029;
        const int SOME_ID = 8036; 


        /// <summary>
        /// Parses the events logs from the auditLogPaths paths into CiEvent objects
        /// </summary>
        /// <param name="auditLogPaths"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadArbitraryEventLogs(List<string> auditLogPaths)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            List<SignerEvent> ciSignerEvents = new List<SignerEvent>();
            List<SignerEvent> appSignerEvents = new List<SignerEvent>();


            foreach (var auditLogPath in auditLogPaths)
            {
                // Check that path provided is an evtx log
                if (Path.GetExtension(auditLogPath) != ".evtx")
                {
                    continue; // do something here? Log?
                }

                EventLogReader log = new EventLogReader(auditLogPath, PathType.FilePath);

                // Read Signature Events first to prepopulate for correlation with audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    if (entry.Id == SIG_INFO_ID) // CI Signature events
                    {
                        SignerEvent signerEvent = ReadSignatureEvent(entry);
                        if(signerEvent != null)
                        {
                            ciSignerEvents.Add(signerEvent);
                        }
                    }
                    else if(entry.Id == APP_SIG_INFO_ID) // AppLocker Eignature events
                    {
                        SignerEvent signerEvent = ReadAppLockerSignatureEvent(entry);
                        if (signerEvent != null)
                        {
                            appSignerEvents.Add(signerEvent);
                        }
                    }
                }

                log = new EventLogReader(auditLogPath, PathType.FilePath);
                // Read all other audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    // Audit 3076's
                    if (entry.Id == AUDIT_PE_ID)
                    {
                        List<CiEvent> auditEvents = ReadPEAuditEvent(entry, ciSignerEvents);

                        foreach(CiEvent auditEvent in auditEvents)
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
                    // Block 3077's
                    else if(entry.Id == BLOCK_PE_ID)
                    {
                        List<CiEvent> blockEvents = ReadPEBlockEvent(entry, ciSignerEvents);
                        foreach (CiEvent blockEvent in blockEvents)
                        {
                            if (blockEvent != null)
                            {
                                if (!IsDuplicateEvent(blockEvent, ciEvents))
                                {
                                    ciEvents.Add(blockEvent);
                                }
                            }
                        }
                    }

                    // AppLocker MSI and Script channel
                    else if (entry.Id == AUDIT_SCRIPT_ID || entry.Id == BLOCK_SCRIPT_ID)
                    {
                        List<CiEvent> appEvents = ReadAppLockerEvents(entry, appSignerEvents);
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
                    else if(entry.Id == BLOCK_SIG_LEVEL_ID)
                    {
                        List<CiEvent> blockSLEvents = ReadSLBlockEvent(entry, ciSignerEvents);

                        foreach(CiEvent blockSLEvent in blockSLEvents)
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
        /// Parses the events logs from the auditLogPaths paths into CiEvent objects
        /// </summary>
        /// <param name="auditLogPaths"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadSystemEventLogs()
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            List<SignerEvent> ciSignerEvents = new List<SignerEvent>();

            List<string> logPaths = new List<string>();
            logPaths.Add("Microsoft-Windows-CodeIntegrity/Operational");
            logPaths.Add("Microsoft-Windows-Applocker/MSI and Script");

            // Loop through the CI and AppLocker logs
            foreach (var logPath in logPaths)
            {
                EventLogReader log = new EventLogReader(logPath, PathType.LogName);

                // Read Signature Events first to prepopulate for correlation with audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    if (entry.Id == SIG_INFO_ID)
                    {
                        SignerEvent signerEvent = ReadSignatureEvent(entry);
                        if (signerEvent != null)
                        {
                            ciSignerEvents.Add(signerEvent);
                        }
                    }
                }

                log = new EventLogReader(logPath, PathType.LogName);
                // Read all other audit/block events
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    // Audit 3076's
                    if (entry.Id == AUDIT_PE_ID)
                    {
                        List<CiEvent> auditEvents = ReadPEAuditEvent(entry, ciSignerEvents);

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
                    // Block 3077's
                    else if (entry.Id == BLOCK_PE_ID)
                    {
                        List<CiEvent> blockEvents = ReadPEBlockEvent(entry, ciSignerEvents);
                        foreach (CiEvent blockEvent in blockEvents)
                        {
                            if (blockEvent != null)
                            {
                                if (!IsDuplicateEvent(blockEvent, ciEvents))
                                {
                                    ciEvents.Add(blockEvent);
                                }
                            }
                        }
                    }

                    // Block 3033's
                    else if (entry.Id == BLOCK_SIG_LEVEL_ID)
                    {
                        List<CiEvent> blockSLEvents = ReadSLBlockEvent(entry, ciSignerEvents);

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
        /// Parses the EventRecord for a 3078 audit event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadPEAuditEvent(EventRecord entry, List<SignerEvent> ciSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Event Info
            // Version 4
            try
            {
                ciEvent.EventId = entry.Id;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);
                ciEvent.SHA1 = (byte[])entry.Properties[12].Value;
                ciEvent.SHA1Page = (byte[])entry.Properties[8].Value;
                ciEvent.SHA2 = (byte[])entry.Properties[14].Value;
                ciEvent.SHA2Page = (byte[])entry.Properties[10].Value;
                ciEvent.OriginalFilename = entry.Properties[24].Value.ToString();
                ciEvent.InternalFilename = entry.Properties[26].Value.ToString();
                ciEvent.FileDescription = entry.Properties[28].Value.ToString();
                ciEvent.ProductName = entry.Properties[30].Value.ToString();
                ciEvent.FileVersion = entry.Properties[31].Value.ToString();

                // Policy related info
                // ciEvent.PolicyGUID = entry.Properties[32].Value.ToString();
                ciEvent.PolicyName = entry.Properties[18].Value.ToString();
                ciEvent.PolicyId = entry.Properties[20].Value.ToString();
                ciEvent.PolicyHash = (byte[])entry.Properties[22].Value;

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in ciSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerInfo = signer;
                        ciEvents.Add(ciEvent);
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
                return null;
            }

            return ciEvents;
        }

        /// <summary>
        /// Parses the EventRecord for a 3077 block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadPEBlockEvent(EventRecord entry, List<SignerEvent> ciSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent(); 

            // Version 5
            // Event Info
            try
            {
                ciEvent.EventId = entry.Id;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);
                ciEvent.SHA1 = (byte[])entry.Properties[12].Value;
                ciEvent.SHA1Page = (byte[])entry.Properties[8].Value;
                ciEvent.SHA2 = (byte[])entry.Properties[14].Value;
                ciEvent.SHA2Page = (byte[])entry.Properties[10].Value;
                ciEvent.OriginalFilename = entry.Properties[24].Value.ToString();
                ciEvent.InternalFilename = entry.Properties[26].Value.ToString();
                ciEvent.FileDescription = entry.Properties[28].Value.ToString();
                ciEvent.ProductName = entry.Properties[30].Value.ToString();
                ciEvent.FileVersion = entry.Properties[31].Value.ToString();

                // Policy related info
                ciEvent.PolicyGUID = entry.Properties[32].Value.ToString();
                ciEvent.PolicyName = entry.Properties[18].Value.ToString();
                ciEvent.PolicyId = entry.Properties[20].Value.ToString();
                ciEvent.PolicyHash = (byte[])entry.Properties[22].Value;

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in ciSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerInfo = signer;
                        ciEvents.Add(ciEvent);
                    }
                }

                // In the case where the file is unsigned
                if (ciEvents.Count == 0)
                {
                    ciEvents.Add(ciEvent);
                }
            }
            catch(Exception e)
            {
                return null; 
            }

            return ciEvents; 
        }

        /// <summary>
        /// Parses the EventRecord for a 8028 or 8029 Script/MSI audit/block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadAppLockerEvents(EventRecord entry, List<SignerEvent> appSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Version 0
            // Event Info
            try
            {
                ciEvent.EventId = entry.Id;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);
                ciEvent.SHA1 = (byte[])entry.Properties[2].Value;
                ciEvent.SHA1Page = (byte[])entry.Properties[6].Value;
                ciEvent.SHA2 = (byte[])entry.Properties[3].Value;
                ciEvent.SHA2Page = (byte[])entry.Properties[7].Value;

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in appSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerInfo = signer;
                        ciEvents.Add(ciEvent);
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
                return null;
            }

            return ciEvents;
        }


        /// <summary>
        /// Parses the EventRecord for a 3033 signing level block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static List<CiEvent> ReadSLBlockEvent(EventRecord entry, List<SignerEvent> ciSignerEvents)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            CiEvent ciEvent = new CiEvent();

            // Version 0
            // Event Info
            try
            {
                ciEvent.EventId = entry.Id;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FilePath = Helper.GetDOSPath(ntfilePath);
                ciEvent.FileName = Path.GetFileName(ciEvent.FilePath);

                // Try to match with pre-populated signer events
                // This will be the only thing to allow on to by pass this block event
                foreach (SignerEvent signer in ciSignerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerInfo = signer;
                        ciEvents.Add(ciEvent);
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
                return null;
            }

            return ciEvents;
        }

        /// <summary>
        /// Parses the EventRecord for a 3089 signature event into a signer event object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static SignerEvent ReadSignatureEvent(EventRecord entry)
        {
            // Version 4 

            SignerEvent signerEvent = new SignerEvent();

            // Event Info
            try
            {
                signerEvent.EventId = entry.Id;
                signerEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                signerEvent.PublisherName = entry.Properties[14].Value.ToString();
                signerEvent.IssuerName = entry.Properties[16].Value.ToString();
                signerEvent.IssuerTBSHash = (byte[]) entry.Properties[18].Value;

            }
            catch (Exception e)
            {
                return null;
            }

            return signerEvent; 
        }

        /// <summary>
        /// Parses the EventRecord for a 8038 AppLocker signature event into a signer event object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static SignerEvent ReadAppLockerSignatureEvent(EventRecord entry)
        {
            // Version 0 

            SignerEvent signerEvent = new SignerEvent();

            // Event Info
            try
            {
                signerEvent.EventId = entry.Id;
                signerEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                signerEvent.PublisherName = entry.Properties[3].Value.ToString();
                signerEvent.IssuerName = entry.Properties[5].Value.ToString();
                signerEvent.IssuerTBSHash = (byte[])entry.Properties[9].Value;

            }
            catch (Exception e)
            {
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
            byte[] fileHash = newEvent.SHA2;
            string filePath = newEvent.FilePath; // could be same file but different path
            int eventId = newEvent.EventId;

            string publisher = newEvent.SignerInfo.PublisherName;

            // Check policy hash - could be blocked by different policies in the evtx
            byte[] policyHash = newEvent.PolicyHash; 

            foreach(CiEvent existingEvent in existingEvents)
            {
                if(fileHash == existingEvent.SHA2 &&
                    filePath == existingEvent.FilePath && 
                    policyHash == existingEvent.PolicyHash &&
                    publisher == existingEvent.SignerInfo.PublisherName &&
                    eventId == existingEvent.EventId)
                {
                    return true; 
                }

            }

            return false; 
        }
    }

    public class CiEvent
    {
        /// <summary>
        /// Event related inffo
        /// </summary>
        public int EventId { get; set; }
        public string CorrelationId { get; set; }

        // File related info
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] SHA1 { get; set; }
        public byte[] SHA1Page { get; set; }
        public byte[] SHA2 { get; set; }
        public byte[] SHA2Page { get; set; }

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
            this.SignerInfo = new SignerEvent();
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
    }
}
