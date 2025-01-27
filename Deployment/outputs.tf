output "webapp_name" {
  value =  module.webapp_service.webapp_name
}

output "mock_webapp_name" {
  value = local.env_name == "dev" ? local.mock_web_app_name : null
}

output "resource_group" {
  value = azurerm_resource_group.rg.name
}

output "keyvault_uri" {
  value = module.key_vault.keyvault_uri
}

output "webapp_slot_name" {
  value = module.webapp_service.slot_name
}

output "webapp_slot_default_site_hostname" {
  value = module.webapp_service.slot_default_site_hostname
}

output "webapp_default_site_hostname" {
  value = azurerm_linux_web_app.webapp_service.default_hostname
}
