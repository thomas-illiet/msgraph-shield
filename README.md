# MSGraph-Shield Solution

[![.NET Core](https://github.com/thomas-illiet/msgraph-shield/actions/workflows/windows_build.yml/badge.svg)](https://github.com/thomas-illiet/msgraph-shield/actions/workflows/windows_build.yml)

> Microsoft Graph Proxy Server authorizations: another layer of abstraction!

## Overview

MSGraph Shield empowers you to establish a robust permission layer for your company. With its intuitive rule configuration, you'll harness the full power of the shield engine with every request. This ensures both the agility of your company and the protection of internal data.

## Features

* ✂️ **Flexible:** Base on [Micro Rule Engine](https://github.com/runxc1/MicroRuleEngine/)
* 🤝 **Compatible:** Works with all Microsoft Graph API Servers.
* 🎯 **Per-Type or Per-Field:** Write permissions for your schema, types or specific fields (check the example below).

## EF Core & Data Access

The solution uses these `DbContexts`:

* `DataConfigDbContext`: for product configuration store
* `DataProtectionDbContext`: for dataprotection

### Run entity framework migrations:

> NOTE: Initial migrations are a part of the repository.

  - It is possible to use powershell script in folder `build/add-migrations.ps1`.
  - This script take two arguments:
    - --migration (migration name)
    - --migrationProviderName (provider type - available choices: All, SqlServer, MySql, PostgreSQL)
  
- For example: 
`.\add-migrations.ps1 -migration DbInit -migrationProviderName SqlServer`

### Available database providers:

- SqlServer
- MySql
- PostgreSQL

> It is possible to switch the database provider via `appsettings.json`:
```json
"DatabaseProviderConfiguration": {
    "ProviderType": "SqlServer" 
}
```

### Connection strings samples for available db providers

**PostgreSQL**: 
> Server=localhost;Port=5432;Database=IdentityServer4Admin;User Id=sa;Password=#;

**MySql:** 
> server=localhost;database=IdentityServer4Admin;user=root;password=#

## Contributing

We are always looking for people to help us grow `msgraphql-shield`! If you have an issue, feature request, or pull request, let us know!

## License

MIT @ Thomas ILLIET