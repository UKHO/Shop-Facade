Write-Host "========================================="
Write-Host "AGENT DIAGNOSTICS"
Write-Host "========================================="

Write-Host "`n=== Agent Information ==="
Write-Host "Agent Name: $env:AGENT_NAME"
Write-Host "Agent Machine Name: $env:AGENT_MACHINENAME"
Write-Host "Agent OS: $env:AGENT_OS"
Write-Host "Agent Pool: $env:AGENT_POOL"

Write-Host "`n=== Network Information ==="
# Get hostname
$hostname = hostname
Write-Host "Hostname: $hostname"

# Get IP addresses
try {
    $ips = [System.Net.Dns]::GetHostAddresses($hostname) | Where-Object { $_.AddressFamily -eq 'InterNetwork' }
    Write-Host "IP Addresses: $($ips.IPAddressToString -join ', ')"
} catch {
    Write-Host "Could not retrieve IP addresses: $($_.Exception.Message)"
}

# Get default gateway (Linux-specific, best effort)
if ($IsLinux) {
    Write-Host "`n=== Network Routes ==="
    try {
        $routes = ip route show | Select-String "default"
        Write-Host $routes
    } catch {
        Write-Host "Could not retrieve routes"
    }
    
    Write-Host "`n=== DNS Configuration ==="
    try {
        Get-Content /etc/resolv.conf | Write-Host
    } catch {
        Write-Host "Could not read DNS config"
    }
} elseif ($IsWindows) {
    Write-Host "`n=== Network Configuration ==="
    Get-NetIPAddress -AddressFamily IPv4 | Select-Object InterfaceAlias, IPAddress, PrefixLength | Format-Table
    
    Write-Host "`n=== DNS Servers ==="
    Get-DnsClientServerAddress -AddressFamily IPv4 | Select-Object InterfaceAlias, ServerAddresses | Format-Table
}

Write-Host "`n========================================="
