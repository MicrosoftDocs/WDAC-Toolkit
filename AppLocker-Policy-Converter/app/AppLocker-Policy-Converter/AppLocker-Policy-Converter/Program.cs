using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AppLocker_Policy_Converter
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowErrorScreen();
                return -1;
            }

            if(HelpRequested(args))
            {
                ShowHelpMenu();
                return 0; 
            }

            if(args.Length < 4)
            {
                ShowErrorScreen();
                return -1;
            }

            // Get list of the AppLocker policy paths
            List<string> applockerPolicyPaths = GetAppLockerPaths(args);
            if(applockerPolicyPaths == null)
            {
                ShowErrorScreen();
                return -1;
            }

            // Get the path to the WDAC xml policy to create
            string outputPath = GetOutputPath(args);
            if(String.IsNullOrEmpty(outputPath))
            {
                ShowErrorScreen();
                return -1;
            }

            List<AppLockerPolicy> appLockerPolicies = ParseAppLockerPolicies(applockerPolicyPaths);
            ConvertPolicies(appLockerPolicies, outputPath);

            return 0; 
        }

        /// <summary>
        /// Checks if the help command is provided. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static bool HelpRequested(string[] args)
        {
            for(int i=0; i < args.Length; i++)
            {
                if(args[i] == "-h" || args[i] == "--help")
                {
                    return true; 
                }
            }

            return false; 
        }

        /// <summary>
        /// Returns the list of AppLocker policy paths input in the cmdline
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static List<string> GetAppLockerPaths(string[] args)
        {
            List<string> appLockerPaths = new List<string>();
            int appLockerArgV = -1;
            int outputPathArgV = -1;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-PolicyPaths")
                {
                    appLockerArgV = i; 
                }
                
                if(args[i] == "-OutputPath")
                {
                    outputPathArgV = i; 
                }
            }

            // If either of these were not set, the command line is missing a parameter
            if(appLockerArgV == -1 || outputPathArgV == -1)
            {
                return null; 
            }

            // Provided in order, e.g. -InputPaths <path_1>,<path_2> -OutputPath <path_3>
            if(appLockerArgV < outputPathArgV)
            {
                for(int j = appLockerArgV + 1; j < outputPathArgV; j++)
                {
                    appLockerPaths.Add(args[j]); 
                }
            }
            // Provided in order, e.g. -OutputPath <path_1> -InputPaths <path_2>,<path_3>
            else
            {
                for (int j = appLockerArgV + 1; j < args.Length; j++)
                {
                    appLockerPaths.Add(args[j]);
                }
            }

            return appLockerPaths; 
        }

        /// <summary>
        /// Parses the input paths to extract the 'Output Path' from the AppLocker policy paths
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static string GetOutputPath(string[] args)
        {
            string outputPath = string.Empty;
            int outputPathArgV = -1; 

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-OutputPath")
                {
                    outputPathArgV = i;
                    break; 
                }
            }

            if(outputPathArgV == -1 || outputPathArgV == args.Length - 1)
            {
                return null; 
            }

            // Nothing provided for InputPath
            if(args[outputPathArgV+1] == "-PolicyPaths")
            {
                return null; 
            }
            else
            {
                outputPath = args[outputPathArgV + 1];
            }

            return outputPath; 
        }

        /// <summary>
        /// Parses the AppLocker paths provided by the user and adds them to the list to process only if the file exists
        /// </summary>
        /// <param name="policyPaths"></param>
        static List<AppLockerPolicy> ParseAppLockerPolicies(List<string> policyPaths)
        {
            List<AppLockerPolicy> appLockerPolicies = new List<AppLockerPolicy>(); 
            foreach (string policyPath in policyPaths)
            {
                if(File.Exists(policyPath))
                {
                    appLockerPolicies.Add(Helper.SerializeAppLockerPolicy(policyPath));
                }
            }

            return appLockerPolicies; 
        }

        /// <summary>
        /// Calls the ProcessPolicy method on the list of AppLocker policies provided
        /// </summary>
        /// <param name="appLockerPolicies"></param>
        /// <param name="outputPath"></param>
        static void ConvertPolicies(List<AppLockerPolicy> appLockerPolicies, string outputPath)
        {
            List<SiPolicy> wdacPolicies = new List<SiPolicy>();
            foreach(AppLockerPolicy appLockerPolicy in appLockerPolicies)
            {
                ProcessPolicy(appLockerPolicy, outputPath);
            }
        }
        
        /// <summary>
        /// Iterates through the list of AppLocker rules and converts and adds each to a WDAC policy
        /// </summary>
        /// <param name="appLockerPolicy"></param>
        /// <param name="outputPath"></param>
        static void ProcessPolicy(AppLockerPolicy appLockerPolicy, string outputPath)
        {
            SiPolicy wdacPolicy = Helper.DeserializeXMLStringtoPolicy(Properties.Resources.Empty);

            foreach(RuleCollectionType ruleCollection in appLockerPolicy.RuleCollection)
            {
                for(int i = 0; i < ruleCollection.Items.Length; i++)
                {
                    if(ruleCollection.Items[i].GetType() == typeof(FilePublisherRuleType))
                    {
                       wdacPolicy = Helper.ConvertFilePublisherRule((FilePublisherRuleType)ruleCollection.Items[i], wdacPolicy);
                    }
                    else if (ruleCollection.Items[i].GetType() == typeof(FileHashRuleType))
                    {
                       wdacPolicy = Helper.ConvertFileHashRule((FileHashRuleType)ruleCollection.Items[i], wdacPolicy);
                    }
                    else if(ruleCollection.Items[i].GetType() == typeof(FilePathRuleType))
                    {
                        wdacPolicy = Helper.ConvertFilePathRule((FilePathRuleType)ruleCollection.Items[i], wdacPolicy);
                    }
                }
            }

            wdacPolicy = FormatPolicy(wdacPolicy);
            Helper.SerializePolicytoXML(wdacPolicy, outputPath); 
        }

        /// <summary>
        /// Sets new PolicyID and BasePolicyID, as well as the FriendlyName on the SiPolicy signing scenarios
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        static SiPolicy FormatPolicy(SiPolicy siPolicy)
        {
            // Set GUIDs
            Guid newGuid = new Guid();
            siPolicy.BasePolicyID = newGuid.ToString();
            siPolicy.PolicyID = siPolicy.BasePolicyID;

            // Set Signing Scenario friendly names
            siPolicy.SigningScenarios[0].FriendlyName = "Auto generated policy on " + Helper.GetFormattedDate();
            siPolicy.SigningScenarios[1].FriendlyName = siPolicy.SigningScenarios[0].FriendlyName; 

            return siPolicy;
        }

        /// <summary>
        /// Shows a default error screen on the console
        /// </summary>
        static void ShowErrorScreen()
        {
            Console.WriteLine("");
            Console.WriteLine("ERROR(S):");
            Console.WriteLine("Invalid arguments.");
            ShowHelpMenu();
        }

        /// <summary>
        /// Shows the help menu on the console
        /// </summary>
        static void ShowHelpMenu()
        {
            Console.WriteLine("-h  | --help       Displays this help screen.");
            Console.WriteLine("-PolicyPaths       Required parameter. Path(s) to the AppLocker policies to convert.");
            Console.WriteLine("-OutputPath        Required parameter. Path to the converted WDAC policy.");
        }
    }
}
