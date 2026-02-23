locals {
  env_name               = lower(terraform.workspace)
  service_name           = "shopfacade"
  web_app_name           = "${local.service_name}-${local.env_name}-api-webapp"
  adds_mock_web_app_name = "${local.service_name}-${local.env_name}-adds-mock-webapp"
  key_vault_name         = "${local.service_name}-${local.env_name}-kv"
  tags = {
    SERVICE          = "Shop Facade"
    ENVIRONMENT      = local.env_name
    SERVICE_OWNER    = "UKHO"
    RESPONSIBLE_TEAM = "Abzu"
    CALLOUT_TEAM     = "On-Call_N/A"
    COST_CENTRE      = "A.011.15.12"
  }
  pe_identity = "${local.service_name}${local.env_name}"
  vnet_link   = "${local.service_name}${local.env_name}"

  # Private endpoint connections
  private_connection_webapp      = "/subscriptions/${var.subscription_id}/resourceGroups/shopfacade-${local.env_name}-rg/providers/Microsoft.Web/sites/shopfacade-${local.env_name}-api-webapp"
  private_connection_keyvault    = "/subscriptions/${var.subscription_id}/resourceGroups/m-spokeconnect-rg/providers/Microsoft.KeyVault/vaults/${local.service_name}${local.env_name}-kv"
  private_connection_keyvault_ex = "/subscriptions/${var.subscription_id}/resourceGroups/m-spokeconnect-rg/providers/Microsoft.KeyVault/vaults/${local.service_name}${local.env_name}-kv-ex"
  private_connection_storage     = "/subscriptions/${var.subscription_id}/resourceGroups/m-spokeconnect-rg/providers/Microsoft.Storage/storageAccounts/shopfacade${local.env_name}sa"

  # Zone groups for DNS integration
  zone_group_webapp      = "${local.service_name}${local.env_name}webapp-zone"
  zone_group_keyvault    = "${local.service_name}${local.env_name}kv-zone"
  zone_group_keyvault_ex = "${local.service_name}${local.env_name}kv-ex-zone"
  zone_group_storage     = "${local.service_name}${local.env_name}storage-zone"

  # Legacy variables for backwards compatibility
  private_connection = local.private_connection_webapp
  zone_group         = local.zone_group_webapp
  dns_resource_group = var.dns_zone_rg
}
