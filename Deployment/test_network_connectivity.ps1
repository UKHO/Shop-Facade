Param(
    [Parameter(Mandatory=$true)][string]$targetUrl
)

# Remove protocol prefix if present
$hostname = $targetUrl -replace 'https?://', ''

Write-Host "Testing connectivity to: $hostname"

# DNS Resolution Test
Write-Host "`n=== DNS Resolution Test ==="
try {
    $dns = Resolve-DnsName $hostname -ErrorAction Stop
    Write-Host "✓ DNS resolved to: $($dns.IPAddress -join ', ')"
} catch {
    Write-Error "✗ DNS resolution failed: $($_.Exception.Message)"
    throw "DNS resolution failed for $hostname"
}

# TCP Connectivity Test
Write-Host "`n=== TCP Connectivity Test (Port 443) ==="
$tcpTest = Test-NetConnection -ComputerName $hostname -Port 443 -WarningAction SilentlyContinue

if ($tcpTest.TcpTestSucceeded) {
    Write-Host "✓ TCP port 443 is reachable"
} else {
    Write-Error "✗ Cannot reach TCP port 443 - Check NSG rules and private endpoint configuration"
    throw "TCP connectivity failed for $hostname on port 443"
}

Write-Host "`n✓ Network connectivity verified successfully"
