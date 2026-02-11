Param(
    [Parameter(Mandatory=$true)][string]$targetUrl
)

# Remove protocol prefix if present
$hostname = $targetUrl -replace 'https?://', ''

Write-Host "Testing connectivity to: $hostname"

# DNS Resolution Test (Cross-platform)
Write-Host "`n=== DNS Resolution Test ==="
try {
    $ipAddresses = [System.Net.Dns]::GetHostAddresses($hostname)
    if ($ipAddresses) {
        Write-Host "✓ DNS resolved to: $($ipAddresses.IPAddressToString -join ', ')"
    } else {
        throw "No IP addresses returned"
    }
} catch {
    Write-Error "✗ DNS resolution failed: $($_.Exception.Message)"
    throw "DNS resolution failed for $hostname"
}

# TCP Connectivity Test (Cross-platform using .NET)
Write-Host "`n=== TCP Connectivity Test (Port 443) ==="
try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $connection = $tcpClient.BeginConnect($hostname, 443, $null, $null)
    $wait = $connection.AsyncWaitHandle.WaitOne(5000, $false)
    
    if ($wait) {
        $tcpClient.EndConnect($connection)
        $tcpClient.Close()
        Write-Host "✓ TCP port 443 is reachable"
    } else {
        $tcpClient.Close()
        Write-Error "✗ Cannot reach TCP port 443 - Connection timeout"
        throw "TCP connectivity timeout for $hostname on port 443"
    }
} catch {
    Write-Error "✗ Cannot reach TCP port 443 - Check NSG rules and private endpoint configuration"
    Write-Error "Error details: $($_.Exception.Message)"
    throw "TCP connectivity failed for $hostname on port 443"
}

Write-Host "`n✓ Network connectivity verified successfully"
