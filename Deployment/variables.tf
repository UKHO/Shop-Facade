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
