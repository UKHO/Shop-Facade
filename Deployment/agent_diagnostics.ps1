Param(
    [Parameter(Mandatory=$false)][string]$poolName = "Unknown",
    [Parameter(Mandatory=$false)][string]$poolDemands = "None"
)

Write-Host "========================================="
Write-Host "AGENT DIAGNOSTICS"
Write-Host "========================================="

Write-Host "`n=== Agent Pool Configuration ==="
Write-Host "Pool Name: $poolName"
Write-Host "Pool Demands: $poolDemands"

Write-Host "`n=== Agent Information ==="
Write-Host "Agent Name: $env:AGENT_NAME"
Write-Host "Agent Machine Name: $env:AGENT_MACHINENAME"
Write-Host "Agent OS: $env:AGENT_OS"
Write-Host "Agent Pool: $poolName"
Write-Host "Agent ID: $env:AGENT_ID"
Write-Host "Agent Job Name: $env:AGENT_JOBNAME"
Write-Host "Agent Working Directory: $env:AGENT_WORKFOLDER"

Write-Host "`n=== Pipeline Environment Variables ==="
Get-ChildItem env: | Where-Object { $_.Name -like "AGENT_*" -or $_.Name -like "BUILD_*" -or $_.Name -like "SYSTEM_*" } | 
    Sort-Object Name | 
    ForEach-Object { Write-Host "$($_.Name) = $($_.Value)" }

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
