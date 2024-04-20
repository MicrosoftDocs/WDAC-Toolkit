using System;
using System.IO;
using Microsoft.Win32; 
using System.Diagnostics; 

namespace WDAC_Wizard
{
    public class Logger
    {
        public StreamWriter _Log;
        public string FileName;

        public static Logger Log { get; private set; } 
        // Singleton pattern here we only allow one instance of the class. 

        /// <summary>
        /// Logger constructor. Creates new single instance of Log class
        /// </summary>
        /// <param name="folderPath">Folder location to save log file</param>
        private Logger(string folderPath)
        {
            string fileName = GetLoggerDst();
            this.FileName = folderPath + fileName;

            if (!File.Exists(this.FileName))
            {
                this._Log = new StreamWriter(this.FileName);
            }

            this._Log.AutoFlush = true;
            this.AddBoilerPlate();
        }

        

        public static void NewLogger(string folderPath)
        {
            Logger.Log = new Logger(folderPath); 
        }

        /// <summary>
        /// Adds line to log file of type INFO 
        /// </summary>
        /// <param name="info">Info string to add to log file</param>
        public void AddInfoMsg(string info)
        {
            string msg = String.Format("{0} [INFO]: {1}", Helper.GetFormattedDateTime(), info);
            this._Log.WriteLine(msg);
        }

        /// <summary>
        /// Adds line to log file of type ERROR
        /// </summary>
        /// <param name="error">Error string to add to log file</param>
        public void AddErrorMsg(string error)
        {
            string msg = String.Format("{0} [ERROR]: {1}", Helper.GetFormattedDateTime(), error);
            this._Log.WriteLine(msg);
        }

        /// <summary>
        /// Adds line to log file of type ERROR with exception
        /// </summary>
        /// <param name="error">Error string to add to log file</param>
        /// <param name="e">Exception string to append to log file</param>
        public void AddErrorMsg(string error, Exception e)
        {
            string msg = String.Format("{0} [ERROR]: {1}: {2}", Helper.GetFormattedDateTime(), error, e.ToString());
            this._Log.WriteLine(msg);
        }

        /// <summary>
        /// Adds line to log file of type WARNING
        /// </summary>
        /// <param name="warning">Warning string to add to log file</param>
        public void AddWarningMsg(string warning)
        {
            string msg = String.Format("{0} [WARNING]: {1}", Helper.GetFormattedDateTime(), warning);
            this._Log.WriteLine(msg);
        }

        /// <summary>
        /// Adds a formatted separation line to the log file for readability
        /// </summary>
        /// <param name="subTitle">Subtitle string to add into the separation line</param>
        public void AddNewSeparationLine(string subTitle)
        {
            string[] msg = new string[3];
            msg[0] = String.Format("{0} [INFO]: **********************************************************************", Helper.GetFormattedDateTime());
            msg[1] = String.Format("{0} [INFO]: {1}", Helper.GetFormattedDateTime(), subTitle);
            msg[2] = String.Format("{0} [INFO]: **********************************************************************", Helper.GetFormattedDateTime());

            foreach (var line in msg)
            {
                this._Log.WriteLine(line);
            }
        }

        /// <summary>
        /// Sets the name for the log file based on date and time
        /// </summary>
        /// <returns></returns>
        public string GetLoggerDst()
        {
            return String.Format("/Log_{0}_{1}.txt", Helper.GetFormattedDate(), Helper.GetFormattedTime());
        }

        /// <summary>
        /// Closes the Log StreamWriter object
        /// </summary>
        public void CloseLogger()
        {
            //this._Log.Flush();
            this._Log.Close();
        }

        /// <summary>
        /// Adds boiler plate information on the WDAC Wizard and the OS to the 
        /// top of the log file for diagnostics
        /// </summary>
        private void AddBoilerPlate()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.AddInfoMsg(String.Format("WDAC Policy Wizard Version # {0}", versionInfo.FileVersion));
            string[] winInfo = GetInstallTime(); 
            this.AddInfoMsg(String.Format("Session ID: {0}", winInfo[0]));
            this.AddInfoMsg(String.Format("Windows Version: {0}", winInfo[1]));
        }

        /// <summary>
        /// Retrieves information about the current version of Windows and install time
        /// for the boiler plate
        /// </summary>
        /// <returns></returns>
        private string[] GetInstallTime()
        {
            RegistryHive rootNode = RegistryHive.LocalMachine;
            RegistryView registryView = RegistryView.Registry64;
            RegistryKey root = RegistryKey.OpenBaseKey(rootNode, registryView);
            RegistryKey registryKey = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            RegistryValueKind subKeyValueKind = registryKey.GetValueKind("InstallTime");
            object installTimeValue = registryKey.GetValue("InstallTime");
            object buildLabEx = registryKey.GetValue("BuildLabEx"); 

            return new string[] {installTimeValue.ToString(), buildLabEx.ToString()};
        }
    }
}
