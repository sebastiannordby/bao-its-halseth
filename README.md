# CIS - Custom Install Software


[![CodeQL Analysis](https://img.shields.io/static/v1?label=CodeQL%20Analysis&message=Passing&color=brightgreen)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/codeql-analysis.yml)

[![Unit & Integration Tests](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/testing.yml/badge.svg)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/testing.yml)

[![Deploy to Azure](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/deploy_azure.yml/badge.svg)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/deploy_azure.yml)

## From Legacy to Cloud - Integration between Shopify and SAP

Welcome to the CIS project! This repository documents the journey of transitioning a legacy system into a cloud-based solution, focusing on seamless integration between Shopify and SAP. Below, you'll find an overview of the project, its objectives, and key features.

## Project Overview
This project involves the migration of a desktop application, originally written in Visual Basic, to a cloud-based solution. The new application, built using Blazor, features a modern and professional frontend designed with Tailwind CSS and Radzen Blazor.

## Key Features
- Unit Testing, Integration Testing, and Code Analysis: The application is developed with a strong emphasis on testing and code quality.
- HTTP APIs for Integration: The application now leverages HTTP APIs for seamless integration between Shopify and SAP, replacing the previous CSV-based communication method.
- Interactive UI: The user interface allows for visualization of sales and management of data such as customers and products.
- Migration Routine: A comprehensive migration routine is in place. Upon startup, the application checks if migration has occurred. This process involves uploading the old database to the new database server, mapping from the old data model to the new one, and providing real-time logging to the client via SignalR to track progress.



## Deployment to Azure using Terraform
This project leverages Terraform, an open-source Infrastructure as Code (IaC) tool, to automate the deployment of the application to Azure. This ensures a consistent and repeatable process, reducing the risk of human error and speeding up the deployment process.

### Workflow
The deployment process begins with the creation of a Docker image of the application. This image is then pushed to the Azure Container Registry (ACR), ensuring that the application is ready for deployment.

### Terraform Execution
Upon successful creation and storage of the Docker image, Terraform is executed. The execution of Terraform scripts results in the creation of the following resources in Azure:

App Service: A fully managed platform for building, deploying, and scaling web apps. This is where our application resides.
Database Server: A managed relational database service where our application’s data is stored.
Two Databases: These databases are created within the database server to store and manage the application’s data.
Storage Account: This is used to store the Terraform state, providing a reliable way to track and manage the resources created by Terraform.
This automated deployment process ensures that the application is consistently deployed to Azure, reducing manual effort and increasing efficiency. It also allows for easy scaling and management of the application and its associated resources.


**This project signifies a significant leap from a legacy Visual Basic application to a modern, cloud-based solution, demonstrating the power and potential of cloud computing.
**


