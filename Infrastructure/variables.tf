variable "db_server_username" {
  description = "Username to database server"
  type = string
}

variable "db_server_password" {
  description = "Password to database server"
  type = string
}

variable "acr_username" {
  description = "Username for Container registry"
  type = string
}

variable "acr_password" {
  description = "Password for Container registry"
  type = string
}

variable "resource_group_name" {
  description = "Name for resource group"
  type = string
  default = "cis"
}
