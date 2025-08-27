# Test script to verify ClosedXML Excel export functionality
$url = "https://localhost:7033/api/reports/export-excel"
$outputFile = "test_closedxml_report.xlsx"

try {
    Write-Host "Testing ClosedXML Excel export from: $url"
    
    # Add the service point manager to skip SSL verification for localhost
    add-type @"
        using System.Net;
        using System.Security.Cryptography.X509Certificates;
        public class TrustAllCertsPolicy : ICertificatePolicy {
            public bool CheckValidationResult(
                ServicePoint srvPoint, X509Certificate certificate,
                WebRequest request, int certificateProblem) {
                return true;
            }
        }
"@
    [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
    
    # Download the Excel file
    Invoke-WebRequest -Uri $url -OutFile $outputFile
    
    if (Test-Path $outputFile) {
        $fileSize = (Get-Item $outputFile).Length
        Write-Host "SUCCESS: Excel file created with ClosedXML: $outputFile ($fileSize bytes)"
        
        # Keep the file for manual inspection
        Write-Host "File saved as $outputFile for manual verification"
        Write-Host "You can open this file in Excel to verify the 5 sheets were created correctly"
    } else {
        Write-Host "ERROR: Excel file was not created"
    }
} catch {
    Write-Host "ERROR: $($_.Exception.Message)"
    Write-Host "Full exception: $($_.Exception)"
}
