provider "azuread" {
  # Optionally configure your Azure AD provider here
}

variable "env_name" {
  description = "The environment name (e.g., dev, iat, prod)"
  type        = string
}

variable "api_sp_prefix" {
  description = "Prefix for the API service principal display name"
  type        = string
  default     = "S-100-PERMIT-SERVICE-"
}

variable "target_sp_prefix" {
  description = "Prefix for the target service principal display name"
  type        = string
  default     = "shopfacade-"
}

data "azuread_service_principal" "api" {
  display_name = "${var.api_sp_prefix}${var.env_name}"
}

data "azuread_service_principal" "target" {
  display_name = "${var.target_sp_prefix}${var.env_name}-api-webapp"
}

locals {
  permit_service_user_role_id = [
    for role in data.azuread_service_principal.api.app_roles : role.id
    if role.value == "PermitServiceUser"
  ][0]
}

resource "azuread_app_role_assignment" "assign" {
  count               = var.env_name == "iat" ? 1 : 0
  principal_object_id = data.azuread_service_principal.target.object_id
  app_role_id         = local.permit_service_user_role_id
  resource_object_id  = data.azuread_service_principal.api.object_id
}
