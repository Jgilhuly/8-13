# RestaurantOps Configuration Management Guide

## Overview
This document outlines the secure configuration management implementation for the RestaurantOps application, addressing the critical security vulnerability of hardcoded credentials.

## Configuration Hierarchy

The application uses the following configuration hierarchy (highest priority first):

1. **Environment Variables** (Production)
2. **User Secrets** (Development)
3. **appsettings.{Environment}.json** (Environment-specific)
4. **appsettings.json** (Base configuration)

## Database Connection Configuration

### Development Environment
For local development, the application uses **User Secrets** to store sensitive configuration:

```bash
# Initialize user secrets (already done)
dotnet user-secrets init

# Set the connection string with credentials
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=RestaurantOps;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
```

### Production Environment
For production deployments, use the `RESTAURANTOPS_DB_CONNECTION` environment variable:

```bash
# Linux/macOS
export RESTAURANTOPS_DB_CONNECTION="Server=prod-server;Database=RestaurantOps;User Id=prod_user;Password=secure_password;TrustServerCertificate=True;"

# Windows
set RESTAURANTOPS_DB_CONNECTION=Server=prod-server;Database=RestaurantOps;User Id=prod_user;Password=secure_password;TrustServerCertificate=True;
```

### Docker Deployment
```dockerfile
ENV RESTAURANTOPS_DB_CONNECTION="Server=db-server;Database=RestaurantOps;User Id=app_user;Password=${DB_PASSWORD};TrustServerCertificate=True;"
```

### Azure App Service
Configure the connection string in the Azure portal under **Configuration > Connection strings**.

## Configuration Files

### appsettings.json (Base)
Contains non-sensitive default configuration:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=RestaurantOps;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

### appsettings.Development.json
Development-specific logging configuration:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### appsettings.Production.json
Production-specific configuration with minimal logging:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Default": ""
  },
  "AllowedHosts": "*"
}
```

## Security Implementation

### Environment Variable Override
The application checks for environment variables on startup and overrides configuration accordingly:

```csharp
// Program.cs - Secure configuration management
var environmentConnectionString = Environment.GetEnvironmentVariable("RESTAURANTOPS_DB_CONNECTION");
if (!string.IsNullOrEmpty(environmentConnectionString))
{
    builder.Configuration["ConnectionStrings:Default"] = environmentConnectionString;
    app.Logger.LogInformation("[Config] Using connection string from environment variable");
}
```

### Benefits
1. **Security**: No credentials in source control
2. **Flexibility**: Different configurations per environment
3. **Compliance**: Meets security best practices
4. **Deployment**: Easy to configure in CI/CD pipelines

## Migration from Legacy Configuration

### Before (❌ Security Risk)
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=RestaurantOps;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
  }
}
```

### After (✅ Secure)
- **Development**: Credentials in User Secrets
- **Production**: Credentials in environment variables
- **Source Control**: Only contains non-sensitive defaults

## Troubleshooting

### Connection Issues
1. Verify environment variable is set correctly
2. Check User Secrets configuration in development
3. Validate connection string format
4. Ensure database server is accessible

### Configuration Not Loading
1. Check environment name (`ASPNETCORE_ENVIRONMENT`)
2. Verify file naming conventions
3. Validate JSON syntax
4. Check file permissions

## Best Practices

1. **Never commit credentials** to source control
2. **Use least privilege** database accounts
3. **Rotate credentials** regularly
4. **Monitor access** to configuration stores
5. **Use managed identities** when available (Azure)

## Future Enhancements

1. **Azure Key Vault** integration for enterprise deployments
2. **Configuration encryption** at rest
3. **Centralized configuration** management
4. **Automated credential rotation**
