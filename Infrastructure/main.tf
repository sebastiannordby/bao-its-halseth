data "azurerm_resource_group" "cis" {
  name = var.resource_group_name
}

data "azurerm_container_registry" "acr" {
  name = "cisapplication"
  resource_group_name = data.azurerm_resource_group.cis.name
}

resource "azurerm_mssql_server" "db_server" {
  name = "cis-db-server"
  resource_group_name = data.azurerm_resource_group.cis.name
  location = data.azurerm_resource_group.cis.location
  version = "12.0"
  administrator_login = var.db_server_username
  administrator_login_password = var.db_server_password
}

resource "azurerm_sql_database" "cis" {
  name = "CIS"
  resource_group_name = data.azurerm_resource_group.cis.name
  location = data.azurerm_resource_group.cis.location
  server_name = azurerm_mssql_server.db_server.name
  edition = "Standard"
  collation = "SQL_Latin1_General_CP1_CI_AS"
}

resource "azurerm_sql_database" "swn" {
  name = "swn_distro"
  resource_group_name = data.azurerm_resource_group.cis.name
  location = data.azurerm_resource_group.cis.location
  server_name = azurerm_mssql_server.db_server.name
  edition = "Standard"
  collation = "Danish_Norwegian_CI_AS"
}

resource "azurerm_app_service_plan" "cisappserviceplan" {
  name = "cisappservice-plan"
  location = data.azurerm_resource_group.cis.location
  resource_group_name = data.azurerm_resource_group.cis.name
  kind = "linux"
  reserved = true

  sku {
    tier = "Basic"
    size = "B1"
  }
}

locals {
  connection_string_cis = "Server=${azurerm_mssql_server.db_server.fully_qualified_domain_name};Database=${azurerm_sql_database.cis.name};User ID=${azurerm_mssql_server.db_server.administrator_login};Password=${azurerm_mssql_server.db_server.administrator_login_password};MultipleActiveResultSets=True;TrustServerCertificate=True;"
  connection_string_swn = "Server=${azurerm_mssql_server.db_server.fully_qualified_domain_name};Database=${azurerm_sql_database.swn.name};User ID=${azurerm_mssql_server.db_server.administrator_login};Password=${azurerm_mssql_server.db_server.administrator_login_password};MultipleActiveResultSets=True;TrustServerCertificate=True;"
}

resource "azurerm_linux_web_app" "cisappservice" {
  name = "cis-appservice"
  location = data.azurerm_resource_group.cis.location
  resource_group_name = data.azurerm_resource_group.cis.name
  service_plan_id = azurerm_app_service_plan.cisappserviceplan.id

  site_config {
    always_on = "false"

    application_stack {
      docker_image_name = "cisapp:latest"
      docker_registry_url = "https://${data.azurerm_container_registry.acr.login_server}"
      docker_registry_username = var.acr_username
      docker_registry_password = var.acr_password
    }
  }

  app_settings = {
    "ConnectionStrings__DefaultConnection" = local.connection_string_cis
    "ConnectionStrings__LegacyConnection" = local.connection_string_swn
  }
}
