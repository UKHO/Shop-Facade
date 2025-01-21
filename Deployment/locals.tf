locals {
  env_name           = lower(terraform.workspace)
  service_name       = "shopfacade"  
  web_app_name       = "${local.service_name}-${local.env_name}-api-webapp"
  mock_web_app_name  = "${local.service_name}-${local.env_name}-mock-webapp"
  key_vault_name     = "${local.service_name}-${local.env_name}-kv"
  tags = {
    SERVICE                   = "Shop Facade"
    ENVIRONMENT               = local.env_name
    SERVICE_OWNER             = "UKHO"
    RESPONSIBLE_TEAM          = "Abzu"
    CALLOUT_TEAM              = "On-Call_N/A"
    COST_CENTRE               = "A.011.15.12"
    }
}

