param(
    [Parameter(Mandatory=$true)]
    [string]$workSpace
)

$ErrorActionPreference = "Stop"

Write-Host "=== Phase 1: Deploy Private Endpoints with Local State ===" -ForegroundColor Cyan

# Create temporary backend override to use local state
$backendOverride = @"
terraform {
  backend "local" {
    path = "terraform-pe.tfstate"
  }
}
"@

Write-Host "Creating backend override for local state..."
Set-Content -Path "backend_override.tf" -Value $backendOverride

try {
    # Initialize with local backend
    Write-Host "Initializing Terraform with local backend..."
    terraform init -reconfigure

    # Select/create workspace
    Write-Host "Selecting workspace: $workSpace"
    terraform workspace select $workSpace -or-create 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        terraform workspace new $workSpace
    }

    # Deploy ONLY the private endpoints
    Write-Host "Deploying private endpoints only..."
    terraform apply `
        -target=module.private_endpoint_webapp `
        -target=module.private_endpoint_keyvault `
        -target=module.private_endpoint_keyvault_ex `
        -target=module.private_endpoint_storage `
        -auto-approve

    if ($LASTEXITCODE -ne 0) {
        throw "Private endpoint deployment failed"
    }

    Write-Host "Private endpoints deployed successfully!" -ForegroundColor Green
    
    # Give DNS a moment to propagate
    Write-Host "Waiting 30 seconds for DNS propagation..."
    Start-Sleep -Seconds 30

} finally {
    # Clean up backend override
    if (Test-Path "backend_override.tf") {
        Write-Host "Removing backend override..."
        Remove-Item "backend_override.tf" -Force
    }
}

Write-Host "=== Phase 1 Complete ===" -ForegroundColor Green
Write-Host "Private endpoints are now deployed. Storage account should be accessible via private network."
Write-Host "Proceed with normal Terraform deployment using remote state."
