# .NET 5 Entity Framework Sample (Azure Functions)

## Setup

1. Clone the repo
1. Install the dontnet-ef tool: 
  `dotnet tool install --global dotnet-ef`
1. Set the connection string for migration
  `$env:SqlConnectionString="Server=tcp:my.database.windows.net,1433;......"`
1. Create the initial migration
  `dotnet ef migrations add initial`
1. Update the database
  `dotnet ef database update`