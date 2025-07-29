data "azurerm_subnet" "main_subnet" {
  name                 = var.spoke_subnet_name
  virtual_network_name = var.spoke_vnet_name
  resource_group_name  = var.spoke_rg
}

module "webapp_service" {
  source                    = "./Modules/Webapp"
  name                      = local.web_app_name
  adds_mock_webapp_name     = local.adds_mock_web_app_name
  service_name              = local.service_name                 
  resource_group_name       = azurerm_resource_group.rg.name
  env_name                  = local.env_name
  location                  = azurerm_resource_group.rg.location
  subnet_id                 = data.azurerm_subnet.main_subnet.id
  sku_name                  = var.sku_name[local.env_name]
  app_settings = {
    "KeyVaultSettings__ServiceUri"                              = "https://${local.key_vault_name}.vault.azure.net/"
    "EventHubLoggingConfiguration__Environment"                 = local.env_name
    "EventHubLoggingConfiguration__MinimumLoggingLevel"         = "Warning"
    "EventHubLoggingConfiguration__UkhoMinimumLoggingLevel"     = "Information"
    "ASPNETCORE_ENVIRONMENT"                                    = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                                  = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                           = "true"
    "WEBSITE_ADD_SITENAME_BINDINGS_IN_APPHOST_CONFIG"           = "1"    
  }
  adds_mock_app_settings = {
    "ASPNETCORE_ENVIRONMENT"                                   = local.env_name
    "WEBSITE_RUN_FROM_PACKAGE"                                 = "1"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                          = "true"
  }
  tags                                                          = local.tags
 }

locals {
  kv_read_access_list = {
    "webapp_service"  = module.webapp_service.web_app_object_id
    "webapp_slot"     = module.webapp_service.slot_object_id
  }
}

module "app_insights" {
  source              = "./Modules/AppInsights"
  name                = "${local.service_name}-${local.env_name}-insights"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
}

module "eventhub" {
  source              = "./Modules/EventHub"
  name                = "${local.service_name}-${local.env_name}-events"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tags                = local.tags
  env_name            = local.env_name
}  

module "key_vault" {
  source              = "./Modules/KeyVault"
  name                = local.key_vault_name
  resource_group_name = azurerm_resource_group.rg.name
  env_name            = local.env_name
  tenant_id           = module.webapp_service.web_app_tenant_id
  location            = azurerm_resource_group.rg.location
  read_access_objects =  local.kv_read_access_list
  secrets = {
    "EventHubLoggingConfiguration--ConnectionString"            = module.eventhub.log_primary_connection_string
    "EventHubLoggingConfiguration--EntityPath"                  = module.eventhub.entity_path
    "ApplicationInsights--ConnectionString"                     = module.app_insights.connection_string
  }
  tags                                                          = local.tags
}
