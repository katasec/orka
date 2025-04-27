# Set variables
$projectPath = "C:\path\to\Orka.Abstractions\Orka.Abstractions.csproj"
$outputPath = "C:\path\to\output"
$apiKey = "$env:NUGET_API_KEY"
$nugetSource = "https://api.nuget.org/v3/index.json"

# Make sure output directory exists
if (-not (Test-Path -Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
}

# Pack the project
Write-Output "Project Path: $projectPath"
dotnet pack $projectPath --output $outputPath --configuration Release

# Find the generated .nupkg
$nupkg = Get-ChildItem -r -i "*.nupkg" | Select-Object -First 1

if ($nupkg) {
    # Push the package
    ##dotnet nuget push $nupkg.FullName --api-key $apiKey --source $nugetSource
    Write-Output "✅ Package pushed: $($nupkg.FullName)"
} else {
    Write-Output "❌ No package found to push."
}
