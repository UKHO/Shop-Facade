param (
    [Parameter(mandatory=$false)][int]$StartPort = 5000,
    [Parameter(mandatory=$false)][int]$EndPort = 60000
)

$usedPorts = (Get-NetTCPConnection).LocalPort + (Get-NetUDPEndpoint).LocalPort
$nextPort = $StartPort..$EndPort | where { $usedPorts -notcontains $_ } | select -first 1

if ($nextPort -is [int]) {
    Write-Host "Using port $nextPort"
    $url = "http://localhost:$nextPort/graphapi"
    Write-Host "Url: $url"
    Write-Host "##vso[task.setvariable variable=ADDSMockUrl]$url"
    $env:ADDSMockUrl = $url
    Write-Host "ADDSMockUrl: $env:ADDSMockUrl"
    Write-Host "##vso[task.setvariable variable=GraphApiConfiguration.GraphApiBaseUrl]$url"
    Write-Host "GraphApiConfiguration.GraphApiBaseUrl: $($env:GraphApiConfiguration.GraphApiBaseUrl)"
    Write-Host "##vso[task.setvariable variable=Port;isOutput=true]$nextPort"
    $env:Port = $nextPort
    Write-Host "Port: $env:Port"
} else {
    throw "Can't find an available port"
}
