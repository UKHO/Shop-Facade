param(
    [Parameter(Mandatory=$true)]
    [string]$keyVaultName,
    
    [Parameter(Mandatory=$true)]
    [string]$storageAccountName,
    
    [Parameter(Mandatory=$true)]
    [string]$resourceGroupName
)

$ErrorActionPreference = "Stop"

Write-Host "=== Disabling Public Access (Private Endpoint Only) ===" -ForegroundColor Cyan

# Disable public access on Key Vault (private endpoint only)
Write-Host "Disabling public network access on Key Vault: $keyVaultName"
az keyvault update `
    --name $keyVaultName `
    --resource-group $resourceGroupName `
    --public-network-access Disabled

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Failed to disable public access on Key Vault - may need manual cleanup"
}

# Disable public access on Storage Account (private endpoint only)
Write-Host "Disabling public network access on Storage Account: $storageAccountName"
az storage account update `
    --name $storageAccountName `
    --resource-group $resourceGroupName `
    --public-network-access Disabled

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Failed to disable public access on Storage Account - may need manual cleanup"
}

Write-Host "=== Public Access Disabled ===" -ForegroundColor Green
Write-Host "Key Vault and Storage Account are now private endpoint only"
