param([switch]$IncludeProbe)
$ErrorActionPreference='Stop'
function Run-DotNet([string[]]$Arguments) {
    & dotnet @Arguments
    if($LASTEXITCODE){throw "dotnet failed: $Arguments"}
}
Run-DotNet @('run','--project','tests/FarFlow.Checks/FarFlow.Checks.csproj','-c','Release')
$app='artifacts/FarFlow-Windows-x64'
Run-DotNet @('publish','src/FarFlow.Windows/FarFlow.Windows.csproj','-c','Release','-r','win-x64','-p:Platform=x64','-p:PublishProfile=','-o',$app)
Run-DotNet @('publish','src/FarFlow.ModelHost/FarFlow.ModelHost.csproj','-c','Release','-r','win-x64','-o',"$app/ModelHost")
# Include the official redistributable C++ runtime, rather than depending on a developer PC.
$vs=& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath
$crt=Get-ChildItem (Join-Path $vs 'VC/Redist/MSVC') -Directory | Sort-Object Name -Descending |
    ForEach-Object { Get-ChildItem (Join-Path $_.FullName 'x64') -Directory -Filter 'Microsoft.VC*.CRT' -ErrorAction SilentlyContinue } | Select-Object -First 1
if(-not $crt){throw 'Official redistributable Visual C++ CRT was not found in the selected Windows toolchain.'}
Copy-Item (Join-Path $crt.FullName '*.dll') $app
Copy-Item (Join-Path $crt.FullName '*.dll') "$app/ModelHost"
Copy-Item LICENSE,THIRD_PARTY_NOTICES.md $app
Copy-Item licenses "$app/licenses" -Recurse
Copy-Item brand/fonts/ZalandoSans-OFL.txt "$app/licenses/ZalandoSans-OFL.txt"
Copy-Item docs/TESTING-PREVIEW.md "$app/START-HERE.md"
@{sourceCommit=$env:GITHUB_SHA;configuration='Release';runtime='win-x64';dotnet=(dotnet --version);cppRuntime=$crt.Name;builtUtc=[DateTime]::UtcNow.ToString('O')} | ConvertTo-Json | Set-Content "$app/build-info.json"
if($IncludeProbe){
    Run-DotNet @('publish','tests/FarFlow.Probe/FarFlow.Probe.csproj','-c','Release','-r','win-x64','-o','artifacts/probe')
    Copy-Item "$app/ModelHost" 'artifacts/probe/ModelHost' -Recurse
    Copy-Item (Join-Path $crt.FullName '*.dll') artifacts/probe
}
Compress-Archive "$app/*" 'artifacts/Far-Flow-Windows-0.1.0-preview-x64.zip' -Force
$hash=Get-FileHash 'artifacts/Far-Flow-Windows-0.1.0-preview-x64.zip' -Algorithm SHA256
"$($hash.Hash.ToLower())  Far-Flow-Windows-0.1.0-preview-x64.zip" | Set-Content artifacts/SHA256SUMS.txt
