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
            "dev"     =  "P1v2"            
            "vni"     =  "P1v2"
            "iat"     =  "P1v2"
            "prp"     =  "P1v2"     
            "e2e"     =  "P1v2"
            live      =  "P1v2"
            }
}
