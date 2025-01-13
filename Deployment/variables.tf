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
            "dev"     =  "Premium v2"            
            "vni"     =  "Premium v2"
            "iat"     =  "Premium v2"
            "prp"     =  "Premium v2"     
            "e2e"     =  "Premium v2"
            live      =  "Premium v2"
            }
}
