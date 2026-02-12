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
        # Get all routes
        $allRoutes = ip route show
        Write-Host "All Routes:"
        Write-Host $allRoutes
        
        # Highlight default route
        $defaultRoute = $allRoutes | Select-String "default"
        if ($defaultRoute) {
            Write-Host "`nDefault Gateway: $defaultRoute"
        }
        
        # Check for specific subnet routes
        Write-Host "`nChecking for VNet/subnet routes..."
        $vnetRoutes = $allRoutes | Select-String "10\.|172\.(1[6-9]|2[0-9]|3[0-1])\.|192\.168\."
        if ($vnetRoutes) {
            Write-Host "Private network routes found:"
            Write-Host $vnetRoutes
        } else {
            Write-Warning "⚠ No private network routes detected - agent may not have VNet connectivity"
        }
    } catch {
        Write-Host "Could not retrieve routes: $($_.Exception.Message)"
    }
    
    Write-Host "`n=== DNS Configuration ==="
    try {
        $dnsConfig = Get-Content /etc/resolv.conf
        Write-Host $dnsConfig
        
        # Check for Azure private DNS
        if ($dnsConfig -match "168\.63\.129\.16") {
            Write-Host "`n✓ Azure DNS (168.63.129.16) detected - Private DNS zones may be accessible"
        } else {
            Write-Warning "⚠ Azure DNS not detected - Private DNS zones may not resolve correctly"
        }
    } catch {
        Write-Host "Could not read DNS config: $($_.Exception.Message)"
    }
} elseif ($IsWindows) {
    Write-Host "`n=== Network Configuration ==="
    Get-NetIPAddress -AddressFamily IPv4 | Select-Object InterfaceAlias, IPAddress, PrefixLength | Format-Table
    
    Write-Host "`n=== Network Routes ==="
    try {
        $routes = Get-NetRoute -AddressFamily IPv4 | Where-Object { $_.DestinationPrefix -ne "255.255.255.255/32" } | 
            Select-Object DestinationPrefix, NextHop, RouteMetric, InterfaceAlias | 
            Format-Table -AutoSize
        $routes
        
        # Check for private network routes
        $privateRoutes = Get-NetRoute -AddressFamily IPv4 | Where-Object { 
            $_.DestinationPrefix -match '^10\.' -or 
            $_.DestinationPrefix -match '^172\.(1[6-9]|2[0-9]|3[0-1])\.' -or 
            $_.DestinationPrefix -match '^192\.168\.'
        }
        
        if ($privateRoutes) {
            Write-Host "Private network routes found:"
            $privateRoutes | Select-Object DestinationPrefix, NextHop, InterfaceAlias | Format-Table
        } else {
            Write-Warning "⚠ No private network routes detected - agent may not have VNet connectivity"
        }
    } catch {
        Write-Host "Could not retrieve routes: $($_.Exception.Message)"
    }
    
    Write-Host "`n=== DNS Servers ==="
    $dnsServers = Get-DnsClientServerAddress -AddressFamily IPv4
    $dnsServers | Select-Object InterfaceAlias, ServerAddresses | Format-Table
    
    # Check for Azure DNS
    $azureDns = $dnsServers | Where-Object { $_.ServerAddresses -contains "168.63.129.16" }
    if ($azureDns) {
        Write-Host "✓ Azure DNS (168.63.129.16) detected - Private DNS zones may be accessible"
    } else {
        Write-Warning "⚠ Azure DNS not detected - Private DNS zones may not resolve correctly"
    }
}

Write-Host "`n========================================="
