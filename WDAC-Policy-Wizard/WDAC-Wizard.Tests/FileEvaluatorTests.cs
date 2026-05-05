// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using Moq;
using Xunit;
using WDAC_Wizard;

namespace WDAC_Wizard.Tests
{
    public class FileEvaluatorTests
    {
        #region WldpExecutionPolicy Enum Tests

        [Fact]
        public void WldpExecutionPolicy_Blocked_HasValue0()
        {
            Assert.Equal(0, (int)WldpExecutionPolicy.Blocked);
        }

        [Fact]
        public void WldpExecutionPolicy_Allowed_HasValue1()
        {
            Assert.Equal(1, (int)WldpExecutionPolicy.Allowed);
        }

        [Fact]
        public void WldpExecutionPolicy_RequireSandbox_HasValue2()
        {
            Assert.Equal(2, (int)WldpExecutionPolicy.RequireSandbox);
        }

        #endregion

        #region WldpExecutionEvaluationOptions Enum Tests

        [Fact]
        public void WldpExecutionEvaluationOptions_None_HasValue0()
        {
            Assert.Equal(0, (int)WldpExecutionEvaluationOptions.None);
        }

        [Fact]
        public void WldpExecutionEvaluationOptions_ExecuteInInteractiveSession_HasValue1()
        {
            Assert.Equal(1, (int)WldpExecutionEvaluationOptions.ExecuteInInteractiveSession);
        }

        #endregion

        #region FileEvaluationResult Tests

        [Fact]
        public void FileEvaluationResult_GetResultDescription_WhenApiCallFails_ReturnsErrorMessage()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = false,
                ErrorMessage = "File does not exist."
            };

            Assert.Equal("Error: File does not exist.", result.GetResultDescription());
        }

        [Fact]
        public void FileEvaluationResult_GetResultDescription_WhenAllowed_ReturnsAllowed()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Allowed
            };

            Assert.Equal("Allowed", result.GetResultDescription());
        }

        [Fact]
        public void FileEvaluationResult_GetResultDescription_WhenBlocked_ReturnsBlocked()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Blocked
            };

            Assert.Equal("Blocked", result.GetResultDescription());
        }

        [Fact]
        public void FileEvaluationResult_GetResultDescription_WhenRequireSandbox_ReturnsSandboxMessage()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.RequireSandbox
            };

            Assert.Equal("Blocked (Requires sandbox - not available)", result.GetResultDescription());
        }

        [Fact]
        public void FileEvaluationResult_IsAllowed_WhenPolicyAllowed_ReturnsTrue()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Allowed
            };

            Assert.True(result.IsAllowed);
        }

        [Fact]
        public void FileEvaluationResult_IsAllowed_WhenPolicyBlocked_ReturnsFalse()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Blocked
            };

            Assert.False(result.IsAllowed);
        }

        [Fact]
        public void FileEvaluationResult_IsAllowed_WhenApiCallFailed_ReturnsFalse()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = false,
                ExecutionPolicy = null
            };

            Assert.False(result.IsAllowed);
        }

        [Fact]
        public void FileEvaluationResult_IsBlocked_WhenPolicyBlocked_ReturnsTrue()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Blocked
            };

            Assert.True(result.IsBlocked);
        }

        [Fact]
        public void FileEvaluationResult_IsBlocked_WhenRequireSandbox_ReturnsTrue()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.RequireSandbox
            };

            Assert.True(result.IsBlocked);
        }

        [Fact]
        public void FileEvaluationResult_IsBlocked_WhenAllowed_ReturnsFalse()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = true,
                ExecutionPolicy = WldpExecutionPolicy.Allowed
            };

            Assert.False(result.IsBlocked);
        }

        [Fact]
        public void FileEvaluationResult_IsBlocked_WhenApiCallFailed_ReturnsFalse()
        {
            var result = new FileEvaluationResult
            {
                ApiCallSucceeded = false,
                ExecutionPolicy = null
            };

            Assert.False(result.IsBlocked);
        }

        #endregion

        #region FileEvaluator.EvaluateFile Input Validation Tests

        [Fact]
        public void EvaluateFile_NullPath_ReturnsErrorResult()
        {
            var evaluator = new FileEvaluator();

            var result = evaluator.EvaluateFile(null);

            Assert.False(result.ApiCallSucceeded);
            Assert.Contains("null or empty", result.ErrorMessage);
        }

        [Fact]
        public void EvaluateFile_EmptyPath_ReturnsErrorResult()
        {
            var evaluator = new FileEvaluator();

            var result = evaluator.EvaluateFile(string.Empty);

            Assert.False(result.ApiCallSucceeded);
            Assert.Contains("null or empty", result.ErrorMessage);
        }

        [Fact]
        public void EvaluateFile_NonExistentFile_ReturnsErrorResult()
        {
            var evaluator = new FileEvaluator();
            string nonExistentPath = @"C:\NonExistent\Path\FakeFile_12345.exe";

            var result = evaluator.EvaluateFile(nonExistentPath);

            Assert.False(result.ApiCallSucceeded);
            Assert.Contains("does not exist", result.ErrorMessage);
        }

        [Fact]
        public void EvaluateFile_NonExistentFile_SetsFilePathOnResult()
        {
            var evaluator = new FileEvaluator();
            string nonExistentPath = @"C:\NonExistent\Path\FakeFile_12345.exe";

            var result = evaluator.EvaluateFile(nonExistentPath);

            Assert.Equal(nonExistentPath, result.FilePath);
        }

        #endregion

        #region IFileEvaluator Mock Tests

        [Fact]
        public void MockFileEvaluator_CanReturnAllowedResult()
        {
            var mockEvaluator = new Mock<IFileEvaluator>();
            mockEvaluator.Setup(e => e.EvaluateFile(It.IsAny<string>()))
                .Returns(new FileEvaluationResult
                {
                    FilePath = @"C:\test.exe",
                    ApiCallSucceeded = true,
                    ExecutionPolicy = WldpExecutionPolicy.Allowed,
                    HResult = 0
                });

            var result = mockEvaluator.Object.EvaluateFile(@"C:\test.exe");

            Assert.True(result.ApiCallSucceeded);
            Assert.True(result.IsAllowed);
            Assert.False(result.IsBlocked);
        }

        [Fact]
        public void MockFileEvaluator_CanReturnBlockedResult()
        {
            var mockEvaluator = new Mock<IFileEvaluator>();
            mockEvaluator.Setup(e => e.EvaluateFile(It.IsAny<string>()))
                .Returns(new FileEvaluationResult
                {
                    FilePath = @"C:\malware.exe",
                    ApiCallSucceeded = true,
                    ExecutionPolicy = WldpExecutionPolicy.Blocked,
                    HResult = 0
                });

            var result = mockEvaluator.Object.EvaluateFile(@"C:\malware.exe");

            Assert.True(result.ApiCallSucceeded);
            Assert.False(result.IsAllowed);
            Assert.True(result.IsBlocked);
            Assert.Equal("Blocked", result.GetResultDescription());
        }

        [Fact]
        public void MockFileEvaluator_IsApiAvailable_CanBeMocked()
        {
            var mockEvaluator = new Mock<IFileEvaluator>();
            mockEvaluator.Setup(e => e.IsApiAvailable()).Returns(true);

            Assert.True(mockEvaluator.Object.IsApiAvailable());
        }

        [Fact]
        public void MockFileEvaluator_IsApiAvailable_ReturnsFalse_WhenNotSupported()
        {
            var mockEvaluator = new Mock<IFileEvaluator>();
            mockEvaluator.Setup(e => e.IsApiAvailable()).Returns(false);

            Assert.False(mockEvaluator.Object.IsApiAvailable());
        }

        #endregion

        #region FileEvaluator.IsApiAvailable Tests

        [Fact]
        public void IsApiAvailable_ReturnsBoolean()
        {
            var evaluator = new FileEvaluator();

            // Should return true or false without throwing
            bool available = evaluator.IsApiAvailable();

            Assert.IsType<bool>(available);
        }

        #endregion

        #region FileEvaluationRow Tests

        [Fact]
        public void FileEvaluationRow_Properties_CanBeSetAndRetrieved()
        {
            var row = new FileEvaluationRow
            {
                FilePath = @"C:\test.exe",
                Result = "Allowed",
                Status = "Allowed"
            };

            Assert.Equal(@"C:\test.exe", row.FilePath);
            Assert.Equal("Allowed", row.Result);
            Assert.Equal("Allowed", row.Status);
        }

        [Fact]
        public void FileEvaluationRow_DefaultProperties_AreNull()
        {
            var row = new FileEvaluationRow();

            Assert.Null(row.FilePath);
            Assert.Null(row.Result);
            Assert.Null(row.Status);
        }

        #endregion
    }
}
