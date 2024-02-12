@echo off
echo Database Migration Tool
echo Easily Create Entity Framework Migrations
set /p migname= MigrationName:
dotnet ef migrations add %migname% --project .\CIS.Application.csproj --startup-project ..\CIS\CIS.csproj --context CISDbContext
pause