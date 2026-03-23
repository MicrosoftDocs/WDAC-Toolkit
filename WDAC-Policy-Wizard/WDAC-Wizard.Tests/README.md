# WDAC Wizard Unit Tests

This project contains comprehensive unit tests for the WDAC Policy Wizard application.

## Test Coverage

The test suite includes **72 comprehensive unit tests** covering the following areas:

### 1. Helper Utility Tests (HelperTests.cs)
- **IsValidPublisher** - Publisher CN validation (5 tests)
- **FormatPublisherCN** - Publisher name formatting (4 tests)
- **IsValidVersion** - Version string validation (8 tests)
- **CompareVersions** - Version comparison logic (5 tests)
- **IsValidPathRule** - WDAC path rule validation (6 tests)
- **IsValidText** - Text validation (4 tests)
- **FormatSubjectName** - Certificate subject formatting (3 tests)

**Total: 35 tests**

### 2. Policy Business Logic Tests (PolicyTests.cs)
- **UpdateVersion** - Version increment and rollover logic (7 tests)
  - Tests standard incrementing
  - Tests rollover at UInt16.MaxValue (65535)
  - Tests cascading rollovers across version parts
- **EditPathContainsVersionInfo** - Filename version detection (6 tests)
- **HasRuleOption** - Rule option querying (4 tests)
- **PolicyType Enum** - Enum value validation (1 test)
- **Format Enum** - Format enum validation (1 test)

**Total: 19 tests**

### 3. Supporting Class Tests
- **COM Tests** (COMTests) - COM object validation (2 tests)
- **AppID Tests** (AppIDTests) - AppID tagging validation (3 tests)
- **PolicyFileRules Tests** (PolicyFileRulesTests) - File rule type determination (4 tests)

**Total: 9 tests**

### 4. Logger Tests (LoggerTests.cs)
- Logger initialization and file creation
- Info/Error/Warning message logging
- Exception logging with stack traces
- Separation line formatting
- Boilerplate generation
- AutoFlush configuration

**Total: 9 tests**

## Running the Tests

### From Command Line

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "FullyQualifiedName~HelperTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~IsValidVersion_ValidVersion_ReturnsTrue"
```

### From Visual Studio

1. Open the solution in Visual Studio
2. Go to **Test > Test Explorer**
3. Click **Run All** to execute all tests
4. Or right-click individual tests to run them

## Test Framework

- **xUnit** - Primary testing framework
- **Moq** - Mocking library for dependencies
- **.NET 8** - Target framework matching the main application

## Key Testing Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern:
```csharp
[Fact]
public void TestMethod_Scenario_ExpectedResult()
{
    // Arrange - Set up test data
    var input = "test data";

    // Act - Execute the method under test
    var result = MethodUnderTest(input);

    // Assert - Verify the result
    Assert.Equal(expected, result);
}
```

### Test Naming Convention
Tests use the pattern: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `IsValidVersion_ValidVersion_ReturnsTrue`
- `UpdateVersion_MaxLastPart_RollsOverCascades`
- `HasRuleOption_EmptyList_ReturnsFalse`

## Important Notes

### Logger Singleton
The Logger class uses a singleton pattern which can cause issues in test environments. Tests handle this by:
- Creating isolated logger instances per test
- Wrapping logger-dependent code in try-catch where needed
- Cleaning up log files after tests

### Internal Type Access
The test project accesses internal types from the main assembly using the `InternalsVisibleTo` attribute in `AssemblyInfo.cs`:

```csharp
[assembly: InternalsVisibleTo("WDAC-Wizard.Tests")]
```

This allows testing of internal utility classes like `Helper` without making them public.

## Test Results

Current status: **All 72 tests passing ✅**

```
Passed!  - Failed:     0, Passed:    72, Skipped:     0, Total:    72
```

## Future Test Enhancements

Recommended areas for additional test coverage:
1. **EventLog.cs** - Event parsing and correlation logic
2. **PolicyHelper.cs** - Rule generation and policy modification
3. **AdvancedHunting.cs** / **LogAnalytics.cs** - CSV parsing
4. **PSCmdlets.cs** - PowerShell command building (with mocking)

## Contributing

When adding new tests:
1. Follow the AAA pattern
2. Use descriptive test names
3. Test both happy path and edge cases
4. Include negative test cases for validation methods
5. Add comments for complex test scenarios
6. Ensure tests are independent and don't rely on execution order
