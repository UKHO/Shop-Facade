resource "azurerm_service_plan" "app_service_plan" {
  name                = "${var.service_name}-asp"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku_name            = var.sku_name
  os_type             = "Linux"
  tags                = var.tags
}

resource "azurerm_linux_web_app" "webapp_service" {
  name                      = var.name
  location                  = var.location
  resource_group_name       = var.resource_group_name
  service_plan_id           = azurerm_service_plan.app_service_plan.id
  tags                      = var.tags
  virtual_network_subnet_id = var.subnet_id

  site_config {
    application_stack {    
      dotnet_version = "8.0"
    }
    always_on  = true
    ftps_state = "Disabled"
  }
     
  app_settings = var.app_settings

  identity {
    type = "SystemAssigned"
  }

  https_only = true
}

resource "azurerm_linux_web_app_slot" "staging" {
  name                = "staging"
  app_service_id      = azurerm_linux_web_app.webapp_service.id
  tags                = azurerm_linux_web_app.webapp_service.tags 

  site_config {
    application_stack {    
      dotnet_version = "8.0"
    }
    always_on  = true
    ftps_state = "Disabled"
  }

  identity {
    type = "SystemAssigned"
  }

  https_only = true
}

resource "azurerm_linux_web_app" "adds_mock_webapp_service" {
  count               = var.env_name == "dev" ? 1 : 0
  name                = var.adds_mock_webapp_name
  location            = var.location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  tags                = var.tags
  
  site_config {
    application_stack {    
      dotnet_version = "8.0"
    }
    always_on  = true
    ftps_state = "Disabled"
  }
     
  app_settings = merge(var.adds_mock_app_settings, {
    "WEBSITES_PORT" = "8080"
  })

  identity {
    type = "SystemAssigned"
  }

  https_only = false
}
