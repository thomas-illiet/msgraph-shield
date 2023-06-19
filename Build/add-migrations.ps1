param(
    [string] $migration = 'DbInit',
    [string] $migrationProviderName = 'All',
    [string] $targetContext = 'All',
    [string] $projectName = 'MSGraphShield'
)

$currentPath = Get-Location
Set-Location "../src/Service/$projectName.Api.Service"
Copy-Item appsettings.json -Destination appsettings-backup.json
$settings = Get-Content appsettings.json -raw

# Initialze db context and define the target directory
$targetContexts = @{ 
    DataConfigDbContext     = "Migrations\DataConfig";
    DataProtectionDbContext = "Migrations\DataProtection";
}

# Initialize the db providers and it's respective projects
$dpProviders = @{
    SqlServer  = "..\..\Database\$projectName.Data.SqlServer\$projectName.Data.SqlServer.csproj";
    PostgreSQL = "..\..\Database\$projectName.Data.PostgreSQL\$projectName.Data.PostgreSQL.csproj";
    MySql      = "..\..\Database\$projectName.Data.MySql\$projectName.Data.MySql.csproj";
}

# Fix issue when the tools is not installed and the nuget package does not work see https://github.com/MicrosoftDocs/azure-docs/issues/40048
Write-Host "Updating donet ef tools"
$env:Path += "	% USERPROFILE % \.dotnet\tools";
dotnet tool update --global dotnet-ef

Write-Host "Start migrate projects"
foreach ($provider in $dpProviders.Keys) {

    if ($migrationProviderName -eq 'All' -or $migrationProviderName -eq $provider) {
    
        $projectPath = (Get-Item -Path $dpProviders[$provider] -Verbose).FullName;
        Write-Host "Generate migration for db provider:" $provider ", for project path - " $projectPath

        $providerName = '"ProviderType": "' + $provider + '"'

        $settings = $settings -replace '"ProviderType".*', $providerName
        $settings | set-content appsettings.json
        if ((Test-Path $projectPath) -eq $true) {
            foreach ($context in $targetContexts.Keys) {
                if ($targetContext -eq 'All' -or $context -eq $targetContext) {
                    $migrationPath = $targetContexts[$context];

                    Write-Host "Migrating context " $context
                    dotnet ef migrations add $migration -c $context -o $migrationPath -p $projectPath
                }
            } 
        }
        
    }
}

Remove-Item appsettings.json
Copy-Item appsettings-backup.json -Destination appsettings.json
Remove-Item appsettings-backup.json
Set-Location $currentPath