variable "location" {
  type    = string
  default = "uksouth"
}

variable "resource_group_name" {
  type    = string
  default = "shopfacade"
}

variable "sku_name" {
  type = map(any)
  default = {
            "dev"          =  "P1v2"            
            "vni"          =  "P1v2"
            "iat"          =  "P1v2"
            "pre"          =  "P1v2"     
            "vne"          =  "P1v2"
            "prod"         =  "P1v2"
            }
}

#variable "env_name" {
  #description = "skip Deployment environment (dev)"
  #type        = string
#}

variable "spoke_rg" {
  type = string
}

variable "spoke_vnet_name" {
  type = string
            }

variable "spoke_subnet_name" {
  type = string
}

variable "subscription_id" {
  type = string
}

variable "hub_subscription_id" {
  type = string
}

variable "pe_vnet_name" {
  type = string
}

variable "pe_subnet_name" {
  type = string
}

variable "pe_rg" {
  type = string
}
