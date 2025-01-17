output "webapp_name" {
  value =  module.webapp_service.name
}

output "resource_group" {
  value = azurerm_resource_group.rg.name
}

output "shop_facade_web_app_url" {
  value = "https://${module.webapp_service.default_site_hostname}"
}

output "keyvault_uri" {
  value = module.key_vault.keyvault_uri
}

output "shop_facade_web_app_public_url" {
  value = "https://shopfacade${local.env_name}.admiralty.co.uk"
}
