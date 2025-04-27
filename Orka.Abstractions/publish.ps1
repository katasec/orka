# Set variables
$projectName = $PWD.Path.Split("\")[-1]
$projectPath =  Join-Path $PWD.Path $projectName
$outputPath = Join-Path $PWD.Path "bin" "release"
$apiKey = "$env:NUGET_API_KEY"
$nugetSource = "https://api.nuget.org/v3/index.json"

# Make sure output directory exists
$outputPath = Join-Path $PWD.Path "bin" "release"
if (-not (Test-Path -Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
} else {
    Write-Output "Output directory already exists: $outputPath"
}

$gitTag = git describe --tags --abbrev=0
if (-not $gitTag) {
    Write-Output "❌ No Git tag found!"
    exit 1
}
Write-Output "✅ Using Git tag as version: $gitTag"


# Pack the project
Write-Output "Project Path: $projectPath"
dotnet pack .  --configuration Release /p:PackageVersion=$gitTag

# Find the generated .nupkg
$nupkg = Get-ChildItem -r -i "*.nupkg" | Select-Object -First 1

if ($nupkg) {
    # Push the package
    dotnet nuget push $nupkg.FullName --api-key $apiKey --source $nugetSource
    Write-Output "✅ Package pushed: $($nupkg.FullName)"
} else {
    Write-Output "❌ No package found to push."
}
