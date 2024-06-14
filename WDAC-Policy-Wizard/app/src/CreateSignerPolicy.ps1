# WdacBinPath: path to the WDACWizard.exe; known to be on the disk
# DriverFilePath: path to the binary to be scanned for TBS hash rule
# PolicyPath: path where to create the new policy XML
# Level: Rule level (e.g. Publisher, PcaCertificate,Hash)
param (
    [string]$WdacBinPath,
    [string]$DriverFilePath,
    [string]$PolicyPath,
    [string]$Level,
    [string]$Deny
)

# Expected to fail due to CmdletInvocationException error. Hresult = -2146233087
# This fixes the KeyNotFound error and frees the error for the subsequent New-CiPolicyRule cmd
try
{
    $DummySignerRule = New-CIPolicyRule -Level Publisher -DriverFilePath $WdacBinPath -Fallback Hash
}
catch
{
}

# Run New-CIPolicyRule to generate a rule object from the provided driver file path and specified level (Publisher vs PcaCertificate)
if($Deny -eq "False")
{
    $SignerRule = New-CIPolicyRule -Level $Level -DriverFilePath $DriverFilePath -Fallback Hash -Deny
}
else
{
    $SignerRule = New-CIPolicyRule -Level $Level -DriverFilePath $DriverFilePath -Fallback Hash
}

# Create policy from the SignerRule object
New-CIPolicy -Rules $SignerRule -FilePath $PolicyPath