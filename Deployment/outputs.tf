output "webapp_name" {
  value =  module.webapp_service.webapp_name
}

output "resource_group" {
  value = azurerm_resource_group.rg.name
}

output "keyvault_uri" {
  value = module.key_vault.keyvault_uri
}
