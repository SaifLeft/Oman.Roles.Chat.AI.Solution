# PowerShell script to update localization resources

# Define resource paths
$apiFolder = Join-Path $PSScriptRoot "API"
$resourcesFolder = Join-Path $apiFolder "Resources"

# Create resources folder if it doesn't exist
if (-not (Test-Path $resourcesFolder)) {
    Write-Host "Creating resources folder at: $resourcesFolder"
    New-Item -ItemType Directory -Path $resourcesFolder | Out-Null
}

# Check if ar.json and en.json exist
$arJsonPath = Join-Path $resourcesFolder "ar.json"
$enJsonPath = Join-Path $resourcesFolder "en.json"

$arJson = $null
$enJson = $null

if (Test-Path $arJsonPath) {
    Write-Host "Reading existing ar.json file"
    $arJsonContent = Get-Content -Path $arJsonPath -Raw
    $arJson = ConvertFrom-Json $arJsonContent -AsHashtable
}
else {
    Write-Host "Creating new ar.json file"
    $arJson = @{
        "Errors" = @{}
        "Messages" = @{}
        "Validation" = @{}
    }
}

if (Test-Path $enJsonPath) {
    Write-Host "Reading existing en.json file"
    $enJsonContent = Get-Content -Path $enJsonPath -Raw
    $enJson = ConvertFrom-Json $enJsonContent -AsHashtable
}
else {
    Write-Host "Creating new en.json file"
    $enJson = @{
        "Errors" = @{}
        "Messages" = @{}
        "Validation" = @{}
    }
}

# Scan for localization keys in C# files
Write-Host "Scanning for localization keys in C# files..."
$csFiles = Get-ChildItem -Path $PSScriptRoot -Filter "*.cs" -Recurse
$pattern = '_localizationService\.GetMessage\("([^"]+)",\s*"([^"]+)"'

$foundKeys = @{}

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $matches = [regex]::Matches($content, $pattern)
    
    foreach ($match in $matches) {
        $key = $match.Groups[1].Value
        $category = $match.Groups[2].Value
        
        if (-not $foundKeys.ContainsKey($category)) {
            $foundKeys[$category] = @{}
        }
        
        if (-not $foundKeys[$category].ContainsKey($key)) {
            $foundKeys[$category][$key] = $true
            Write-Host "Found key: $key in category: $category"
        }
    }
}

Write-Host "Found keys in categories: $($foundKeys.Keys.Count)"

# Update resource files with found keys
foreach ($category in $foundKeys.Keys) {
    Write-Host "Processing category: $category"
    
    # Ensure category exists in both language files
    if (-not $arJson.ContainsKey($category)) {
        $arJson[$category] = @{}
    }
    
    if (-not $enJson.ContainsKey($category)) {
        $enJson[$category] = @{}
    }
    
    # Add keys to both language files
    foreach ($key in $foundKeys[$category].Keys) {
        if (-not $arJson[$category].ContainsKey($key)) {
            $arJson[$category][$key] = "[ar] $key"
            Write-Host "Added new key: $key to ar.json in category: $category"
        }
        
        if (-not $enJson[$category].ContainsKey($key)) {
            $enJson[$category][$key] = $key
            Write-Host "Added new key: $key to en.json in category: $category"
        }
    }
}

# Save updated resource files
$arJsonContent = ConvertTo-Json $arJson -Depth 10
$enJsonContent = ConvertTo-Json $enJson -Depth 10

Set-Content -Path $arJsonPath -Value $arJsonContent
Set-Content -Path $enJsonPath -Value $enJsonContent

Write-Host "Resource files updated successfully." 