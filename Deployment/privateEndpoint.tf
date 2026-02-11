data "azurerm_resource_group" "perg" {
  provider = azurerm.shopfacade
  name     = var.spoke_rg
}

data "azurerm_virtual_network" "pevn" {
  provider            = azurerm.shopfacade
  name                = var.shopfacade_vnet_name
  resource_group_name = var.spoke_rg
}

data "azurerm_subnet" "pesn" {
  provider             = azurerm.shopfacade
  name                 = var.shopfacade_subnet_name
  virtual_network_name = var.shopfacade_vnet_name
  resource_group_name  = var.spoke_rg
}

module "private_endpoint_link" {
  source = "github.com/UKHO/tfmodule-azure-private-endpoint-private-link?ref=0.7.1"
  providers = {
    azurerm.hub   = azurerm.hub
    azurerm.spoke = azurerm.shopfacade
  }
  private_connection                              = [local.private_connection]
  zone_group                                      = local.zone_group
  shopfacade_subnet_name_identity                 = [local.shopfacade_subnet_name_identity]
  shopfacade_environment                          = local.env_name
  shopfacade_subnet_name_vnet_rg                  = var.shopfacade_rg
  shopfacade_subnet_name_vnet_name                = var.shopfacade_vnet_name
  shopfacade_subnet_name                          = var.shopfacade_subnet_name
  shopfacade_subnet_name_resource_group           = [azurerm_resource_group.rg.name]
  dns_resource_group                              = local.dns_resource_group
  shopfacade_subnet_name_resource_group_locations = [azurerm_resource_group.rg.location]
}