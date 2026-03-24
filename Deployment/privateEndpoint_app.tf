# Private Endpoint for Application Network Security
# This PE is deployed during main Terraform apply (Devdeploy stage)
# NOT deployed in DevPreDeploy stage

# Web App Private Endpoint
module "private_endpoint_webapp" {
  count  = var.enablePrivateEndpoint ? 1 : 0
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
  dns_zone                    = "privatelink.azurewebsites.net"
}
