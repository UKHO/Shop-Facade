terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.112.0"
    }
  }
  required_version = "=1.10.4"
  backend "azurerm" {
    key                  = "terraform.deployment.tfplan"
    container_name       = "tfstate"
  }
}

provider "azurerm" {
  features {}
}

provider "azurerm" {
  features {} 
  alias = "hub"
  subscription_id = var.hub_subscription_id
}

provider "azurerm" {
  features {} 
  alias = "ps"
  subscription_id = var.subscription_id
}
