
# Homeverse Backend

## Getting Started

To run the project locally, you need to follow these steps:

1. Clone the repository.

```
git clone https://github.com/ducna0610/homeverse-be.git
```

2. Config file appsettings.json

```
{
    "Serilog": {
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft": "Warning",
                "Microsoft.AspNetCore.Hosting.Diagnostics": "Error",
                "Microsoft.Hosting.Lifetime": "Information"
            }
        },
        "WriteTo": [
            {
                "Name": "File",
                "Args": {
                    "path": "D:\\Logs\\log_.txt",
                    "rollingInterval": "Day",
                }
            }
        ],
        "Enrich": [
            "WithMachineName",
            "WithProcessId",
            "WithThreadId"
        ]
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "Database": "...",
        "Redis": "..."
    },
    "JwtSettings": {
        "SecretKey": "...",
        "ExpiryDays": 7,
        "Issuer": "Homeverse",
        "Audience": "Homeverse"
    },
    "MailSettings": {
        "SenderEmail": "...",
        "Password": "..."
    },
    "UrlSettings": {
        "Api": "http://localhost:5004",
        "Frontend": "http://localhost:4200"
    },
    "CloudinarySettings": {
        "CloudName": "..",
        "ApiKey": "...",
        "ApiSecret": "..."
    }
}
```

3. Set up the database, to set up the database you need to follow either one of these steps:

- If you use **Visual Studio**, open the Package Manager Console in **Homeverse.Infrastructure** layer  and run the following command.

```
Update-Database
```

- If you use **Visual Studio Code**, open your terminal in **Homeverse.Infrastructure** layer and run the command.

```
dotnet ef database update --context HomeverseDbContext --startup-project ..\Homeverse.API\Homeverse.API.csproj
```

4. Build and run the application.

## Project Structure

The Homeverse project follows a typical clean architecture structure and TDD with unit test, integration test and automation test.

## Technologies Used

The Homeverse project utilizes the following technologies:

- **Programming Language**: C# 12 (.NET 8)
- **Web Framework**: ASP.NET Core API
- **Database**: SQL server, Entity Framework
- **Cache**: Redis
- **Background Job**: Hangfire
- **File Storage**: Cloundinary
- **Authentication and Authorization**: JSON Web Token (JWT)
- **Testing**: xUnit, AutoFixture, FakeItEasy, Selenium, TestContainer

## License

The Homeverse project is open-source and released under the [MIT License](https://opensource.org/licenses/MIT). You are free to use, modify, and distribute the codebase as per the license terms.