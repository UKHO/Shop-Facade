param(
    [Parameter(Mandatory=$true)]
    [string]$keyVaultName,
    
    [Parameter(Mandatory=$true)]
    [string]$storageAccountName,
    
    [Parameter(Mandatory=$true)]
    [string]$resourceGroupName
)

$ErrorActionPreference = "Stop"

Write-Host "=== Temporarily Enabling Public Access ===" -ForegroundColor Cyan

# Enable public access on Key Vault
Write-Host "Enabling public network access on Key Vault: $keyVaultName"
az keyvault update `
    --name $keyVaultName `
    --resource-group $resourceGroupName `
    --public-network-access Enabled `
    --default-action Allow

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to enable public access on Key Vault"
    exit 1
}

# Enable public access on Storage Account
Write-Host "Enabling public network access on Storage Account: $storageAccountName"
az storage account update `
    --name $storageAccountName `
    --resource-group $resourceGroupName `
    --public-network-access Enabled `
    --default-action Allow

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to enable public access on Storage Account"
    exit 1
}

Write-Host "=== Public Access Enabled ===" -ForegroundColor Green
Write-Host "Key Vault and Storage Account are now accessible from pipeline agents"
