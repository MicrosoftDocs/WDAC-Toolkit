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
            ConvertPolicies(appLockerPolicies);

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
        /// 
        /// </summary>
        /// <param name="policyPaths"></param>
        static List<AppLockerPolicy> ParseAppLockerPolicies(List<string> policyPaths)
        {
            List<AppLockerPolicy> appLockerPolicies = new List<AppLockerPolicy>(); 
            foreach (string policyPath in policyPaths)
            {
                if(File.Exists(policyPath))
                {
                    appLockerPolicies.Add(ParseAppLockerPolicy(policyPath));
                }
            }

            return appLockerPolicies; 
        }

        static AppLockerPolicy ParseAppLockerPolicy(string policyPath)
        {
            AppLockerPolicy policy = Helper.SerializeAppLockerPolicy(policyPath);
            return policy; 
        }

        static void ConvertPolicies(List<AppLockerPolicy> appLockerPolicies)
        {
            List<SiPolicy> wdacPolicies = new List<SiPolicy>();
            foreach(AppLockerPolicy appLockerPolicy in appLockerPolicies)
            {
                ProcessPolicy(appLockerPolicy);
            }
        }

        static void ProcessPolicy(AppLockerPolicy appLockerPolicy)
        {
            Console.WriteLine("Stop");
            SiPolicy wdacPolicy = new SiPolicy(); 

            foreach(RuleCollectionType ruleCollection in appLockerPolicy.RuleCollection)
            {
                for(int i = 0; i < ruleCollection.Items.Length; i++)
                {
                    if(ruleCollection.Items[i].GetType() == typeof(FilePublisherRuleType))
                    {
                        Helper.ConvertFilePublisherRule((FilePublisherRuleType)ruleCollection.Items[i]);
                    }
                    else if (ruleCollection.Items[i].GetType() == typeof(FileHashRuleType))
                    {
                        Helper.ConvertFileHashRule((FileHashRuleType)ruleCollection.Items[i]);
                    }
                    else if(ruleCollection.Items[i].GetType() == typeof(FilePathRuleType))
                    {
                        Helper.ConvertFilePathRule((FilePathRuleType)ruleCollection.Items[i]);
                    }
                }
            }
        }

        static void ShowErrorScreen()
        {
            Console.WriteLine("");
            Console.WriteLine("ERROR(S):");
            Console.WriteLine("Invalid arguments.");
            ShowHelpMenu();
        }

        static void ShowHelpMenu()
        {
            Console.WriteLine("-h  | --help       Displays this help screen.");
            Console.WriteLine("-PolicyPaths       Required parameter. Path(s) to the AppLocker policies to convert.");
            Console.WriteLine("-OutputPath        Required parameter. Path to the converted WDAC policy.");
        }


        public class DriverExceptions
        {
            public const int InvalidPath = 0xE100;
            public static string InvalidPathErrorString = "The path of the file is not valid.";

            public const int NullInfoObj = 0xE101;
            public static string NullInfoObjString = "The file specified cannot be found.";

            public const int EmptyCompanyName = 0xE102;
            public static string EmptyCompanyNameString = "The company name of the PE is empty or null.";

            public const int EmptyOriginalFileName = 0xE103;
            public static string EmptyOriginalFileNameString = "The original filename of the PE is empty or null.";

            public const int BadOriginalFileName = 0xE104;
            public static string BadOriginalFileNameString = "The original filename must contain an extension.";

            public const int EmptyProductName = 0xE105;
            public static string EmptyProductNameString = "The product name of the PE is empty or null.";

            public const int EmptyFileDescription = 0xE106;
            public static string EmptyFileDescriptionString = "The description of the PE is empty or null.";

            public const int EmptyFileVersion = 0xE107;
            public static string EmptyFileVersionString = "The version of the PE is empty or null.";

            public const int BadMajorPartFileVersion = 0xE108;
            public const int BadMinorPartFileVersion = 0xE109;
            public const int BadBuildPartFileVersion = 0xE10A;
            public const int BadPrivatePartFileVersion = 0xE10B;
            public static string BadFileVersionString = "The version of the PE exceeds the upper limit (65535).";

            public const int EmptyProductVersion = 0xE10C;
            public static string EmptyProductVersionString = "The product version of the PE is empty or null.";


            public static string DecodeErrorCode(int errorCode)
            {
                switch (errorCode)
                {
                    case DriverExceptions.NullInfoObj:
                        return DriverExceptions.NullInfoObjString;

                    case DriverExceptions.InvalidPath:
                        return DriverExceptions.InvalidPathErrorString;

                    case DriverExceptions.EmptyCompanyName:
                        return DriverExceptions.EmptyCompanyNameString;

                    case DriverExceptions.EmptyOriginalFileName:
                        return DriverExceptions.EmptyOriginalFileNameString;

                    case DriverExceptions.BadOriginalFileName:
                        return DriverExceptions.BadOriginalFileNameString;

                    case DriverExceptions.EmptyProductName:
                        return DriverExceptions.EmptyProductNameString;

                    case DriverExceptions.EmptyFileDescription:
                        return DriverExceptions.EmptyFileDescriptionString;

                    case DriverExceptions.EmptyFileVersion:
                        return DriverExceptions.EmptyFileVersionString;

                    case DriverExceptions.BadMajorPartFileVersion:
                    case DriverExceptions.BadMinorPartFileVersion:
                    case DriverExceptions.BadBuildPartFileVersion:
                    case DriverExceptions.BadPrivatePartFileVersion:
                        return DriverExceptions.BadFileVersionString;

                    case DriverExceptions.EmptyProductVersion:
                        return DriverExceptions.EmptyProductVersionString;

                    default:
                        return "";
                }
            }
        }
    }
}
