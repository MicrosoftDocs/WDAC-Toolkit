# ScanPath: path to scan
# PolicyPath: path where to create the new policy XML
# Level: Rule level (e.g. Publisher, PcaCertificate,Hash)
# Fallback: comma separated list of fallback options
# PathsToOmit: comma separated list of paths to skip during scanning
# Deny: True/False
# UserPEs: whether to scan for User Mode PEs (True/False)
param (
    [string]$ScanPath,
    [string]$PolicyPath,
    [string]$Level,
    [string]$Fallback,
    [string]$PathsToOmit,
    [string]$Deny,
    [string]$UserPEs
)

# Run New-CIPolicy -Scan to generate a policy from a directory
# The command needs to be run twice to generate the full policy. Otherwise, the "An item with the same key has already been added." WARNING prevents the full policy from being generated.
if($Deny -eq "False")
{
    if($UserPEs -eq "True")
    {
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -UserPEs
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -UserPEs
    }
    else
    {
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit
    }
}
else
{
    if($UserPEs -eq "True")
    {
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -UserPEs -Deny
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -UserPEs -Deny
    }
    else
    {
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -Deny
        New-CIPolicy -ScanPath $ScanPath -Level $Level -FilePath $PolicyPath -Fallback $Fallback -OmitPaths $PathsToOmit -Deny
    }
}