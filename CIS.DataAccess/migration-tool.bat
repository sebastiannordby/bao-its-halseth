@echo off
echo Database Migration Tool
echo Easily Create Entity Framework Migrations
set /p migname= MigrationName:
dotnet ef migrations add %migname% --project .\CIS.DataAccess.csproj --startup-project ..\CIS\CIS.csproj
pause