$ErrorActionPreference='Stop'
New-Item artifacts/fixtures -ItemType Directory -Force | Out-Null
$fixtures=@(
    @{name='en.wav';url='https://huggingface.co/csukuangfj/sherpa-onnx-nemo-parakeet-tdt-0.6b-v3-int8/resolve/2bda32ec70b097a55adaa07d9a7173915b43cc78/test_wavs/en.wav';hash='148b936b43ce7c546a866e64da059f0458aee2d65e617f16e9d94f06e8d99ed6'},
    @{name='nl.wav';url='https://huggingface.co/spaces/k2-fsa/spoken-language-identification/resolve/3c09c87fdfa5e98063dadff5ce3ffea173d0fa76/test_wavs/nl-dutch.wav';hash='3a33a333ed037f07df1ce2cffc1d1c33aa0c3c85821b450202dca9992b19e9c9'}
)
foreach($fixture in $fixtures){
    $file=Join-Path 'artifacts/fixtures' $fixture.name
    Invoke-WebRequest $fixture.url -OutFile $file
    if((Get-FileHash $file -Algorithm SHA256).Hash.ToLower() -ne $fixture.hash){throw 'Speech fixture integrity failed'}
}
$worker=(Resolve-Path artifacts/probe/ModelHost/FarFlow.ModelHost.exe).Path
New-NetFirewallRule -DisplayName 'Far Flow CI offline worker' -Direction Outbound -Program $worker -Action Block | Out-Null
try {
    & artifacts/probe/FarFlow.Probe.exe --audio artifacts/fixtures/en.wav artifacts/fixtures/nl.wav
    if($LASTEXITCODE){throw 'Offline speech smoke failed'}
} finally { Remove-NetFirewallRule -DisplayName 'Far Flow CI offline worker' }
# Downloaded public upstream test fixtures are not included in the app or release assets.
