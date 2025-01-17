output "webapp_name" {
  value = azurerm_linux_web_app.webapp_service.name
}

output "web_app_object_id" {
  value = azurerm_linux_web_app.webapp_service.identity.0.principal_id
}

output "default_site_hostname" {
  value = azurerm_linux_web_app.webapp_service.default_hostname
}


output "web_app_tenant_id" {
  value = azurerm_linux_web_app.webapp_service.identity.0.tenant_id
}
