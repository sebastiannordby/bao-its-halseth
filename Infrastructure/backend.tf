terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "=3.95.0"
    }
  }

  backend "azurerm" {
      resource_group_name = var.backend_resource_group_name 
      storage_account_name = var.backend_storage_account_name 
      container_name = var.backend_container_name 
      key = var.backend_key 
  }
}

provider "azurerm" {
  features {}
}