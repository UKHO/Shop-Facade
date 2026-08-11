terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.80.0"
    }
  }
  required_version = "=1.15.8"
  backend "azurerm" {
    key            = "terraform.deployment.tfplan"
    container_name = "tfstate"
  }
}

provider "azurerm" {
  features {}
}

provider "azurerm" {
  features {}
  alias           = "hub"
  subscription_id = var.hub_subscription_id
}

provider "azurerm" {
  features {}
  alias           = "shopfacade"
  subscription_id = var.subscription_id
}
