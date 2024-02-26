# CIS - Custom Install Software

[![Unit Tests](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/dotnet.yml/badge.svg)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/dotnet.yml)
[![Integration Tests](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/integration-tests.yml/badge.svg)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/integration-tests.yml)
[![CodeQL Analysis](https://img.shields.io/static/v1?label=CodeQL%20Analysis&message=Passing&color=brightgreen)](https://github.com/sebastiannordby/bao-its-halseth/actions/workflows/codeql-analysis.yml)

## From Legacy to Cloud - Integration between Shopify and SAP

Welcome to the CIS project! This repository documents the journey of transitioning a legacy system into a cloud-based solution, focusing on seamless integration between Shopify and SAP. Below, you'll find an overview of the project, its objectives, and key features.

## Project Overview
This project involves the migration of a desktop application, originally written in Visual Basic, to a cloud-based solution. The new application, built using Blazor, features a modern and professional frontend designed with Tailwind CSS and Radzen Blazor.

## Key Features
- Unit Testing, Integration Testing, and Code Analysis: The application is developed with a strong emphasis on testing and code quality.
- HTTP APIs for Integration: The application now leverages HTTP APIs for seamless integration between Shopify and SAP, replacing the previous CSV-based communication method.
- Interactive UI: The user interface allows for visualization of sales and management of data such as customers and products.
- Migration Routine: A comprehensive migration routine is in place. Upon startup, the application checks if migration has occurred. This process involves uploading the old database to the new database server, mapping from the old data model to the new one, and providing real-time logging to the client via SignalR to track progress.

**This project signifies a significant leap from a legacy Visual Basic application to a modern, cloud-based solution, demonstrating the power and potential of cloud computing.
**
