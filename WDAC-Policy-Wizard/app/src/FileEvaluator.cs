// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WDAC_Wizard
{
    /// <summary>
    /// Result of evaluating a single file against the active system WDAC policy.
    /// </summary>
    public class FileEvaluationResult
    {
        public string FilePath { get; set; }
        public bool ApiCallSucceeded { get; set; }
        public WldpExecutionPolicy? ExecutionPolicy { get; set; }
        public int HResult { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Returns a user-friendly description of the evaluation result.
        /// </summary>
        public string GetResultDescription()
        {
            if (!ApiCallSucceeded)
            {
                return $"Error: {ErrorMessage}";
            }

            switch (ExecutionPolicy)
            {
                case WldpExecutionPolicy.Allowed:
                    return "Allowed";
                case WldpExecutionPolicy.Blocked:
                    return "Blocked";
                case WldpExecutionPolicy.RequireSandbox:
                    return "Blocked (Requires sandbox - not available)";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// Returns whether the file is effectively allowed to execute.
        /// </summary>
        public bool IsAllowed => ApiCallSucceeded && ExecutionPolicy == WldpExecutionPolicy.Allowed;

        /// <summary>
        /// Returns whether the file is effectively blocked from execution.
        /// </summary>
        public bool IsBlocked => ApiCallSucceeded &&
            (ExecutionPolicy == WldpExecutionPolicy.Blocked || ExecutionPolicy == WldpExecutionPolicy.RequireSandbox);
    }

    /// <summary>
    /// Maps to the native WLDP_EXECUTION_POLICY enum from wldp.h.
    /// </summary>
    public enum WldpExecutionPolicy
    {
        Blocked = 0,
        Allowed = 1,
        RequireSandbox = 2
    }

    /// <summary>
    /// Maps to the native WLDP_EXECUTION_EVALUATION_OPTIONS enum from wldp.h.
    /// </summary>
    public enum WldpExecutionEvaluationOptions
    {
        None = 0,
        ExecuteInInteractiveSession = 1
    }

    /// <summary>
    /// Interface for file evaluation to support testability via mocking.
    /// </summary>
    public interface IFileEvaluator
    {
        /// <summary>
        /// Evaluates a file against the active system WDAC policy.
        /// </summary>
        FileEvaluationResult EvaluateFile(string filePath);

        /// <summary>
        /// Checks whether the WldpCanExecuteFile API is available on this system.
        /// </summary>
        bool IsApiAvailable();
    }

    /// <summary>
    /// Evaluates files against the active system WDAC policy using the WldpCanExecuteFile API.
    /// Requires Windows 11 Build 22621 or later.
    /// </summary>
    public class FileEvaluator : IFileEvaluator
    {
        // WLDP_HOST_GUID_OTHER: {626C4100-1F76-4FFB-8DD4-84B24E0B1C11}
        private static readonly Guid WLDP_HOST_GUID_OTHER = new Guid("626C4100-1F76-4FFB-8DD4-84B24E0B1C11");

        [DllImport("wldp.dll", EntryPoint = "WldpCanExecuteFile", CharSet = CharSet.Unicode, SetLastError = false)]
        private static extern int NativeWldpCanExecuteFile(
            ref Guid host,
            WldpExecutionEvaluationOptions options,
            SafeFileHandle fileHandle,
            [MarshalAs(UnmanagedType.LPWStr)] string auditInfo,
            out WldpExecutionPolicy result);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        private const uint GENERIC_READ = 0x80000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        /// <inheritdoc/>
        public bool IsApiAvailable()
        {
            // WldpCanExecuteFile requires Windows 11 Build 22621+
            var osVersion = Environment.OSVersion.Version;
            return osVersion.Major >= 10 && osVersion.Build >= 22621;
        }

        /// <inheritdoc/>
        public FileEvaluationResult EvaluateFile(string filePath)
        {
            var result = new FileEvaluationResult { FilePath = filePath };

            if (string.IsNullOrEmpty(filePath))
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = "File path is null or empty.";
                return result;
            }

            if (!File.Exists(filePath))
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = "File does not exist.";
                return result;
            }

            if (!IsApiAvailable())
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = "WldpCanExecuteFile API requires Windows 11 Build 22621 or later.";
                return result;
            }

            SafeFileHandle fileHandle = null;
            try
            {
                // Open a fresh handle per evaluation per API documentation guidance
                fileHandle = CreateFile(
                    filePath,
                    GENERIC_READ,
                    FILE_SHARE_READ,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    FILE_ATTRIBUTE_NORMAL,
                    IntPtr.Zero);

                if (fileHandle.IsInvalid)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    result.ApiCallSucceeded = false;
                    result.HResult = lastError;
                    result.ErrorMessage = $"Could not open file (Win32 error: {lastError}).";
                    return result;
                }

                Guid hostGuid = WLDP_HOST_GUID_OTHER;
                string auditInfo = $"WDAC Policy Wizard: {filePath}";

                int hr = NativeWldpCanExecuteFile(
                    ref hostGuid,
                    WldpExecutionEvaluationOptions.None,
                    fileHandle,
                    auditInfo,
                    out WldpExecutionPolicy executionPolicy);

                if (hr == 0) // S_OK
                {
                    result.ApiCallSucceeded = true;
                    result.ExecutionPolicy = executionPolicy;
                    result.HResult = hr;
                }
                else
                {
                    result.ApiCallSucceeded = false;
                    result.HResult = hr;
                    result.ErrorMessage = $"WldpCanExecuteFile failed with HRESULT: 0x{hr:X8}.";
                }
            }
            catch (DllNotFoundException)
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = "wldp.dll not found. This feature requires Windows 11 Build 22621 or later.";
            }
            catch (EntryPointNotFoundException)
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = "WldpCanExecuteFile not found in wldp.dll. This feature requires Windows 11 Build 22621 or later.";
            }
            catch (Exception ex)
            {
                result.ApiCallSucceeded = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            finally
            {
                if (fileHandle != null && !fileHandle.IsInvalid)
                {
                    fileHandle.Dispose();
                }
            }

            return result;
        }
    }
}
