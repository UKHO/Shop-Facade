output "webapp_name" {
  value = azurerm_linux_web_app.webapp_service.name
}

output "web_app_object_id" {
  value = azurerm_linux_web_app.webapp_service.identity.0.principal_id
}

output "web_app_tenant_id" {
  value = azurerm_linux_web_app.webapp_service.identity.0.tenant_id
}

output "slot_object_id" {
  value = azurerm_linux_web_app_slot.staging.identity.0.principal_id
}

output "slot_tenant_id" {
  value = azurerm_linux_web_app_slot.staging.identity.0.tenant_id
}

output "slot_default_site_hostname" {
  value = azurerm_linux_web_app_slot.staging.default_hostname
}

output "slot_name" {
  value = azurerm_linux_web_app_slot.staging.name
}
