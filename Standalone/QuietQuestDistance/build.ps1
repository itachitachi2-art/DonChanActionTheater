param(
    [string]$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\7 Days To Die"
)

$ErrorActionPreference = "Stop"
$managed = Join-Path $GameDir "7DaysToDie_Data\Managed"
$harmony = Join-Path $GameDir "Mods\0_TFP_Harmony"
$project = Join-Path $PSScriptRoot "QuietQuestDistance.csproj"

dotnet build $project -c Release -p:GameManagedDir="$managed" -p:HarmonyDir="$harmony"
Copy-Item (Join-Path $PSScriptRoot "bin\Release\net48\QuietQuestDistance.dll") $PSScriptRoot -Force
Write-Host "QuietQuestDistance.dll を作成しました。"
