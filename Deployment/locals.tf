locals {
  env_name           = lower(terraform.workspace)
  service_name       = "shopfacade"  
  web_app_name       = "${local.service_name}-${local.env_name}-api-webapp"
  mock_web_app_name  = "${local.service_name}-${local.env_name}-mock-webapp"
  key_vault_name     = "${local.service_name}-${local.env_name}-kv"
  pe_identity        = "${local.service_name}${local.env_name}"
  vnet_link          = "${local.service_name}${local.env_name}"
  private_connection = "/subscriptions/${var.subscription_id}/resourceGroups/ps-${local.env_name}-rg/providers/Microsoft.Web/sites/ps-${local.env_name}-api"
  dns_resource_group = "engineering-rg"
  zone_group         = "${local.service_name}${local.env_name}zone"
  dns_zones          = "privatelink.azurewebsites.net"
  tags = {
    SERVICE                   = "Shop Facade"
    ENVIRONMENT               = local.env_name
    SERVICE_OWNER             = "UKHO"
    RESPONSIBLE_TEAM          = "Abzu"
    CALLOUT_TEAM              = "On-Call_N/A"
    COST_CENTRE               = "A.011.15.12"
    }
}
