# PowerShell script for generating track templates

param(
    [int]$Count = 10,
    [int]$DelaySeconds = 10,
    [string]$ApiKey = "741ed884-ab50-473f-ae29-8000faf9f83d",
    [string]$ConnectionString = "Host=localhost;Database=test",
    [string]$OutputDir = "CarPark.Initializer\Demo\TracksData"
)

Write-Host "Starting generation of $Count track templates..."
Write-Host "Using API key: $ApiKey"
Write-Host "Output directory: $OutputDir"
Write-Host "Delay between generations: $DelaySeconds seconds"
Write-Host ""

if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

for ($i = 1; $i -le $Count; $i++) {
    $guid = [guid]::NewGuid().ToString() + '.json'
    $outputPath = Join-Path $OutputDir $guid
    Write-Host "Generating template $i/$Count with GUID: $guid"

    & dotnet exec CarPark.TrackGenerator/bin/Debug/net9.0/CarPark.TrackGenerator.dll generate-template `
        --output $outputPath `
        --connection-string $ConnectionString `
        --graphhopper-key $ApiKey `
        --center-lat 55,7558 `
        --center-lon 37,6176 `
        --radius-km 300 `
        --min-target-length-km 150 `
        --max-target-length-km 400 `
        --max-speed 120 `
        --min-speed 10 `
        --max-acceleration 12000 `
        --point-interval 30 `
        --interval-variation 10

    if ($i -lt $Count) {
        Write-Host "Waiting $DelaySeconds seconds before next generation..."
        Start-Sleep -Seconds $DelaySeconds
    }
}

Write-Host ""
Write-Host "Generation completed! Generated $Count track templates."
Get-ChildItem $OutputDir *.json | Measure-Object | ForEach-Object { Write-Host "Total files in $OutputDir : $($_.Count)" }