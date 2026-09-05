param(
    [string]$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die"
)

$ErrorActionPreference = "Stop"
$managed = Join-Path $GameDir "7DaysToDie_Data\Managed"
$harmony = Join-Path $GameDir "Mods\0_TFP_Harmony"
dotnet build (Join-Path $PSScriptRoot "DonChanActionTheater.csproj") -c Release -p:GameManagedDir="$managed" -p:HarmonyDir="$harmony"
Copy-Item (Join-Path $PSScriptRoot "bin\Release\net48\DonChanActionTheater.dll") $PSScriptRoot -Force
Write-Host "DonChanActionTheater.dll を作成しました。"
