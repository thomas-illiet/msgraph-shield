param(
    [string] $old,
    [string] $new
)

$oldVersion = "<Version>$old</Version>"
$newVersion = "<Version>$new</Version>"

$oldVersionNuspec = "<version>$old</version>"
$newVersionNuspec = "<version>$new</version>"

$projectFiles = Get-ChildItem .\..\Src -include *.csproj -Recurse
foreach ($file in $projectFiles) {
    Write-Host $file.PSPath

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace $oldVersion, $newVersion } |
    Set-Content $file.PSPath -Encoding UTF8
}