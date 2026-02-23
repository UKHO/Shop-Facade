data "azurerm_resource_group" "perg" {
  provider = azurerm.shopfacade
  name     = var.spoke_rg
}

data "azurerm_virtual_network" "pevn" {
  provider            = azurerm.shopfacade
  name                = var.pe_vnet_name
  resource_group_name = var.spoke_rg
}

data "azurerm_subnet" "pesn" {
  provider             = azurerm.shopfacade
  name                 = var.pe_subnet_name
  virtual_network_name = var.pe_vnet_name
  resource_group_name  = var.spoke_rg
}

# Web App Private Endpoint
module "private_endpoint_webapp" {
  count  = contains(["prod", "pre"], local.env_name) ? 0 : 1
  source = "github.com/UKHO/tfmodule-azure-private-endpoint-private-link?ref=0.7.1"
  providers = {
    azurerm.hub   = azurerm.hub
    azurerm.spoke = azurerm.shopfacade
  }
  private_connection          = [local.private_connection_webapp]
  zone_group                  = local.zone_group_webapp
  pe_identity                 = ["${local.pe_identity}-webapp"]
  pe_environment              = local.env_name
  pe_vnet_rg                  = var.spoke_rg
  pe_vnet_name                = var.pe_vnet_name
  pe_subnet_name              = var.pe_subnet_name
  pe_resource_group           = [azurerm_resource_group.rg.name]
  dns_resource_group          = local.dns_resource_group
  pe_resource_group_locations = [azurerm_resource_group.rg.location]
}

# Key Vault Private Endpoint
module "private_endpoint_keyvault" {
  count  = contains(["prod", "pre"], local.env_name) ? 0 : 1
  source = "github.com/UKHO/tfmodule-azure-private-endpoint-private-link?ref=0.7.1"
  providers = {
    azurerm.hub   = azurerm.hub
    azurerm.spoke = azurerm.shopfacade
  }
  private_connection          = [local.private_connection_keyvault]
  zone_group                  = local.zone_group_keyvault
  pe_identity                 = ["${local.pe_identity}-kv-tf"]
  pe_environment              = local.env_name
  pe_vnet_rg                  = var.spoke_rg
  pe_vnet_name                = var.pe_vnet_name
  pe_subnet_name              = var.pe_subnet_name
  pe_resource_group           = [local.pe_resource_group]
  dns_resource_group          = local.dns_resource_group
  pe_resource_group_locations = [local.pe_location]
  subresource_names           = ["vault"]
}

# Key Vault External Private Endpoint
module "private_endpoint_keyvault_ex" {
  count  = contains(["prod", "pre"], local.env_name) ? 0 : 1
  source = "github.com/UKHO/tfmodule-azure-private-endpoint-private-link?ref=0.7.1"
  providers = {
    azurerm.hub   = azurerm.hub
    azurerm.spoke = azurerm.shopfacade
  }
  private_connection          = [local.private_connection_keyvault_ex]
  zone_group                  = local.zone_group_keyvault_ex
  pe_identity                 = ["${local.pe_identity}-kv-ex-tf"]
  pe_environment              = local.env_name
  pe_vnet_rg                  = var.spoke_rg
  pe_vnet_name                = var.pe_vnet_name
  pe_subnet_name              = var.pe_subnet_name
  pe_resource_group           = [local.pe_resource_group]
  dns_resource_group          = local.dns_resource_group
  pe_resource_group_locations = [local.pe_location]
  subresource_names           = ["vault"]
}

# Storage Account Private Endpoint
module "private_endpoint_storage" {
  count  = contains(["prod", "pre"], local.env_name) ? 0 : 1
  source = "github.com/UKHO/tfmodule-azure-private-endpoint-private-link?ref=0.7.1"
  providers = {
    azurerm.hub   = azurerm.hub
    azurerm.spoke = azurerm.shopfacade
  }
  private_connection          = [local.private_connection_storage]
  zone_group                  = local.zone_group_storage
  pe_identity                 = ["${local.pe_identity}-storage-tf"]
  pe_environment              = local.env_name
  pe_vnet_rg                  = var.spoke_rg
  pe_vnet_name                = var.pe_vnet_name
  pe_subnet_name              = var.pe_subnet_name
  pe_resource_group           = [local.pe_resource_group]
  dns_resource_group          = local.dns_resource_group
  pe_resource_group_locations = [local.pe_location]
  subresource_names           = ["blob"]
}