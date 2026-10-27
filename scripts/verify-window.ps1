param(
    [Parameter(Mandatory=$true)][string]$ExePath,
    [Parameter(Mandatory=$true)][string]$EvidenceDirectory,
    [string]$Arguments = ''
)
$ErrorActionPreference = 'Stop'
New-Item $EvidenceDirectory -ItemType Directory -Force | Out-Null
$process = Start-Process (Resolve-Path $ExePath) -ArgumentList $(if ($Arguments) {$Arguments} else {'--window-smoke'}) -PassThru
try {
    $deadline = [DateTime]::UtcNow.AddSeconds(50)
    do {
        Start-Sleep -Milliseconds 500
        $process.Refresh()
        if ($process.HasExited) { throw "App exited before showing a window: $($process.ExitCode)" }
    } while ($process.MainWindowHandle -eq 0 -and [DateTime]::UtcNow -lt $deadline)
    if ($process.MainWindowHandle -eq 0 -or -not $process.Responding) { throw 'No responsive top-level window' }
    Start-Sleep -Seconds 3
    Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class WindowCapture {
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left, Top, Right, Bottom; }
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hwnd);
}
'@
    Add-Type -AssemblyName System.Drawing
    [WindowCapture]::SetForegroundWindow($process.MainWindowHandle) | Out-Null
    Start-Sleep -Seconds 1
    $rect = New-Object WindowCapture+Rect
    [WindowCapture]::GetWindowRect($process.MainWindowHandle,[ref]$rect) | Out-Null
    $width=$rect.Right-$rect.Left; $height=$rect.Bottom-$rect.Top
    if ($width -lt 100 -or $height -lt 100) { throw 'Window dimensions are invalid' }
    $bitmap=New-Object System.Drawing.Bitmap($width,$height)
    $graphics=[System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.CopyFromScreen($rect.Left,$rect.Top,0,0,$bitmap.Size)
        $bitmap.Save((Join-Path (Resolve-Path $EvidenceDirectory) 'window.png'))
    } finally { $graphics.Dispose(); $bitmap.Dispose() }
    @{
        utc=[DateTime]::UtcNow.ToString('O'); title=$process.MainWindowTitle;
        responsive=$process.Responding; hwnd=$process.MainWindowHandle.ToInt64();
        width=$width; height=$height; sourceCommit=$env:GITHUB_SHA;
        environment='GitHub hosted Windows runner; no physical microphone or keyboard certification'
    } | ConvertTo-Json | Set-Content (Join-Path $EvidenceDirectory 'window-evidence.json')
} finally {
    # Ephemeral CI cleanup. Interactive tester sessions remain open until the tester closes them.
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
