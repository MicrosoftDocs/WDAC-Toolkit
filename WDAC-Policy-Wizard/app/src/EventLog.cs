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
        // Audit Events
        const int AUDIT_KERNEL_ID = 3067;
        const int AUDIT_PE_ID = 3076;
        const int AUDIT_SCRIPT_ID = 8028;

        // Block Events
        const int BLOCK_PE_ID = 3077;
        const int BLOCK_KERNEL_ID = 3068;
        const int BLOCK_SCRIPT_ID = 8029;

        const int BLOCK_SIG_LEVEL_ID = 3033;

        // Signature Event
        const int SIG_INFO_ID = 3089;

        public static List<CiEvent> ReadArbitraryEventLogs(List<string> auditLogPaths)
        {
            List<CiEvent> ciEvents = new List<CiEvent>();
            List<SignerEvent> signerEvents = new List<SignerEvent>(); 

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
                    if (entry.Id == SIG_INFO_ID)
                    {
                        SignerEvent signerEvent = ReadSignatureEvent(entry);
                        if(signerEvent != null)
                        {
                            signerEvents.Add(signerEvent);
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
                        CiEvent auditEvent = ReadPEAuditEvent(entry, signerEvents);
                        if (auditEvent != null)
                        {
                            ciEvents.Add(auditEvent);
                        }
                    }
                    // Block 3077's
                    else if(entry.Id == BLOCK_PE_ID)
                    {
                        CiEvent blockEvent = ReadPEAuditEvent(entry, signerEvents);
                        if (blockEvent != null)
                        {
                            ciEvents.Add(blockEvent);
                        }
                    }
                    // Block 3033's
                    else if(entry.Id == BLOCK_SIG_LEVEL_ID)
                    {
                        CiEvent blockSLEvent = ReadSLBlockEvent(entry, signerEvents);
                        if (blockSLEvent != null)
                        {
                            ciEvents.Add(blockSLEvent);
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
        public static CiEvent ReadPEAuditEvent(EventRecord entry, List<SignerEvent> signerEvents)
        {
            CiEvent ciEvent = new CiEvent();

            // Event Info
            // Version 4
            try
            {
                ciEvent.EventId = 3076;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FileName = Helper.GetDOSPath(ntfilePath);
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

                // Try to match with pre-populated signer events
                foreach (SignerEvent signer in signerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerEvents.Add(signer);
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return ciEvent;
        }

        /// <summary>
        /// Parses the EventRecord for a 3077 block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static CiEvent ReadPEBlockEvent(EventRecord entry, List<SignerEvent> signerEvents)
        {
            CiEvent ciEvent = new CiEvent(); 

            // Version 5
            // Event Info
            try
            {
                ciEvent.EventId = 3077;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FileName = Helper.GetDOSPath(ntfilePath);
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

                // Try to match with pre-populated signer events
                foreach(SignerEvent signer in signerEvents)
                {
                    if(signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerEvents.Add(signer);
                    }
                }

            }
            catch(Exception e)
            {
                return null; 
            }

            return ciEvent; 
        }

        /// <summary>
        /// Parses the EventRecord for a 3033 signing level block event into a CiEvent object
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static CiEvent ReadSLBlockEvent(EventRecord entry, List<SignerEvent> signerEvents)
        {
            CiEvent ciEvent = new CiEvent();

            // Version 0
            // Event Info
            try
            {
                ciEvent.EventId = 3033;
                ciEvent.CorrelationId = entry.ActivityId.ToString();

                // File related info
                string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage
                ciEvent.FileName = Helper.GetDOSPath(ntfilePath);

                // Try to match with pre-populated signer events
                // This will be the only thing to allow on to by pass this block event
                foreach (SignerEvent signer in signerEvents)
                {
                    if (signer.CorrelationId == ciEvent.CorrelationId)
                    {
                        ciEvent.SignerEvents.Add(signer);
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }

            return ciEvent;
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
                signerEvent.EventId = 3089;
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

        public List<SignerEvent> SignerEvents {get;set;}

        // Policy related info
        public string PolicyName { get; set; }
        public string PolicyGUID { get; set; }
        public string PolicyId { get; set; }

        public CiEvent()
        {
            this.SignerEvents = new List<SignerEvent>(); 
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
