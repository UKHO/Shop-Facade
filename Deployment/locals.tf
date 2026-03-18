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
  pe_identity       = "${local.service_name}${local.env_name}"
  vnet_link         = "${local.service_name}${local.env_name}"
  pe_resource_group = "m-spokeconnect-rg"
  pe_location       = "uksouth"

  # Private endpoint connection for webapp
  private_connection_webapp = "/subscriptions/${var.subscription_id}/resourceGroups/shopfacade-${local.env_name}-rg/providers/Microsoft.Web/sites/shopfacade-${local.env_name}-api-webapp"

  # Zone group for webapp DNS integration
  zone_group_webapp = "${local.service_name}${local.env_name}webapp-zone"

  # DNS resource group
  dns_resource_group = var.dns_zone_rg
}
