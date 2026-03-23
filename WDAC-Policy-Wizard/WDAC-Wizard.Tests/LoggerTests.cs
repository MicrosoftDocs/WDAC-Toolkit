// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using Xunit;
using WDAC_Wizard;

namespace WDAC_Wizard.Tests
{
    public class LoggerTests
    {
        private readonly string _testLogPath;

        public LoggerTests()
        {
            // Create a temporary directory for test logs
            _testLogPath = Path.Combine(Path.GetTempPath(), "WDACWizardTests");
            if (!Directory.Exists(_testLogPath))
            {
                Directory.CreateDirectory(_testLogPath);
            }
        }

        [Fact]
        public void GetLoggerDst_ReturnsFormattedFileName()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "GetLoggerDst");
            Directory.CreateDirectory(testPath);
            Logger.NewLogger(testPath);

            // Act
            string result = Logger.Log.GetLoggerDst();

            // Assert
            Assert.StartsWith("/Log_", result);
            Assert.EndsWith(".txt", result);
            Assert.Contains("_", result);

            // Cleanup
            Logger.Log.CloseLogger();
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void NewLogger_CreatesLoggerInstance()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "NewLoggerTest");
            Directory.CreateDirectory(testPath);

            // Act
            Logger.NewLogger(testPath);

            // Assert
            Assert.NotNull(Logger.Log);
            Assert.NotNull(Logger.Log.FileName);
            Assert.True(File.Exists(Logger.Log.FileName));

            // Cleanup
            Logger.Log.CloseLogger();
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void AddInfoMsg_WritesToLogFile()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "InfoMsgTest");
            Directory.CreateDirectory(testPath);
            Logger.NewLogger(testPath);
            string testMessage = "This is a test info message";

            // Act
            Logger.Log.AddInfoMsg(testMessage);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(Logger.Log.FileName);
            Assert.Contains("[INFO]:", logContent);
            Assert.Contains(testMessage, logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void AddErrorMsg_WritesToLogFile()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "ErrorMsgTest");
            Directory.CreateDirectory(testPath);
            Logger.NewLogger(testPath);
            string testMessage = "This is a test error message";

            // Act
            Logger.Log.AddErrorMsg(testMessage);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(Logger.Log.FileName);
            Assert.Contains("[ERROR]:", logContent);
            Assert.Contains(testMessage, logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void AddErrorMsgWithException_WritesToLogFile()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "ErrorMsgExceptionTest");
            if (Directory.Exists(testPath))
            {
                Directory.Delete(testPath, true);
            }
            Directory.CreateDirectory(testPath);

            // Force create a new logger instance
            Logger.NewLogger(testPath);
            string testMessage = "This is a test error message";
            Exception testException = new Exception("Test exception");
            string logFileName = Logger.Log.FileName;

            // Act
            Logger.Log.AddErrorMsg(testMessage, testException);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(logFileName);
            Assert.Contains("[ERROR]:", logContent);
            Assert.Contains(testMessage, logContent);
            Assert.Contains("Test exception", logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void AddWarningMsg_WritesToLogFile()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "WarningMsgTest");
            Directory.CreateDirectory(testPath);
            Logger.NewLogger(testPath);
            string testMessage = "This is a test warning message";

            // Act
            Logger.Log.AddWarningMsg(testMessage);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(Logger.Log.FileName);
            Assert.Contains("[WARNING]:", logContent);
            Assert.Contains(testMessage, logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void AddNewSeparationLine_WritesFormattedSeparator()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "SeparationLineTest");
            Directory.CreateDirectory(testPath);
            Logger.NewLogger(testPath);
            string subtitle = "Test Section";

            // Act
            Logger.Log.AddNewSeparationLine(subtitle);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(Logger.Log.FileName);
            Assert.Contains("**********************************************************************", logContent);
            Assert.Contains(subtitle, logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void Logger_WritesBoilerPlate_OnCreation()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "BoilerPlateTest");
            Directory.CreateDirectory(testPath);

            // Act
            Logger.NewLogger(testPath);
            Logger.Log.CloseLogger();

            // Assert
            string logContent = File.ReadAllText(Logger.Log.FileName);
            Assert.Contains("WDAC Policy Wizard Version #", logContent);
            Assert.Contains("Session ID:", logContent);
            Assert.Contains("Windows Version:", logContent);

            // Cleanup
            CleanupTestDirectory(testPath);
        }

        [Fact]
        public void Logger_AutoFlushEnabled()
        {
            // Arrange
            string testPath = Path.Combine(_testLogPath, "AutoFlushTest");
            Directory.CreateDirectory(testPath);

            // Act
            Logger.NewLogger(testPath);

            // Assert
            Assert.True(Logger.Log._Log.AutoFlush);

            // Cleanup
            Logger.Log.CloseLogger();
            CleanupTestDirectory(testPath);
        }

        #region Helper Methods

        private void CleanupTestDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }

        #endregion
    }
}
