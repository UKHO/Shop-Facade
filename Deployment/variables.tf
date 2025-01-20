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
            "vnextiat"     =  "P1v2"
            "iat"          =  "P1v2"
            "preprod"      =  "P1v2"     
            "vnexte2e"     =  "P1v2"
            "live"         =  "P1v2"
            }
}

variable "spoke_rg" {
  type = string
}

variable "spoke_vnet_name" {
  type = string
}

variable "spoke_subnet_name" {
  type = string
}
