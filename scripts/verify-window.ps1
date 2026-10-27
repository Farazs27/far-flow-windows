param(
    [Parameter(Mandatory=$true)][string]$ExePath,
    [Parameter(Mandatory=$true)][string]$EvidenceDirectory,
    [string]$Arguments = '',
    [switch]$VerifyBrand,
    [switch]$WaitForModelReady
)
$ErrorActionPreference = 'Stop'
New-Item $EvidenceDirectory -ItemType Directory -Force | Out-Null
# On the ephemeral runner only, request a useful desktop canvas if the Server display cmdlet is available.
if(Get-Command Set-DisplayResolution -ErrorAction SilentlyContinue){
    try{Set-DisplayResolution -Width 1440 -Height 1000 -Force -ErrorAction Stop | Out-Null}catch{}
}
$resolvedExe = (Resolve-Path $ExePath).Path
$assetsPath=Join-Path ([IO.Path]::GetDirectoryName($resolvedExe)) 'ui-smoke.json'
if(Test-Path $assetsPath){Remove-Item $assetsPath}
$readyPath=Join-Path ([IO.Path]::GetDirectoryName($resolvedExe)) 'model-ready.json'
if(Test-Path $readyPath){Remove-Item $readyPath}
$process = Start-Process $resolvedExe -WorkingDirectory ([IO.Path]::GetDirectoryName($resolvedExe)) -ArgumentList $(if ($Arguments) {$Arguments} else {'--window-smoke'}) -PassThru
try {
    $deadline = [DateTime]::UtcNow.AddSeconds(50)
    do {
        Start-Sleep -Milliseconds 500
        $process.Refresh()
        if ($process.HasExited) { throw "App exited before showing a window: $($process.ExitCode)" }
    } while ($process.MainWindowHandle -eq 0 -and [DateTime]::UtcNow -lt $deadline)
    if ($process.MainWindowHandle -eq 0 -or -not $process.Responding) { throw 'No responsive top-level window' }
    Start-Sleep -Seconds 3
    if($WaitForModelReady){
        $readyDeadline=[DateTime]::UtcNow.AddSeconds(45)
        while(-not(Test-Path $readyPath) -and [DateTime]::UtcNow -lt $readyDeadline){
            Start-Sleep -Milliseconds 200
            $process.Refresh();if($process.HasExited){throw 'App exited while loading the model'}
        }
        if(-not(Test-Path $readyPath)){throw 'The actual app did not reach model-ready state'}
        $modelReady=Get-Content $readyPath | ConvertFrom-Json
        if(-not $modelReady.modelReady){throw 'Model readiness was not confirmed'}
        Copy-Item $readyPath (Join-Path $EvidenceDirectory 'model-ready.json')
    }
    Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class WindowCapture {
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left, Top, Right, Bottom; }
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
  [DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(IntPtr hwnd,int attribute,out Rect rect,int size);
  public static Rect VisibleBounds(IntPtr hwnd) {if(DwmGetWindowAttribute(hwnd,9,out Rect r,Marshal.SizeOf<Rect>())!=0)GetWindowRect(hwnd,out r);return r;}
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hwnd);
  private delegate bool EnumProc(IntPtr hwnd, IntPtr parameter);
  [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, IntPtr parameter);
  [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
  [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
  [DllImport("user32.dll",CharSet=CharSet.Unicode)] private static extern int GetWindowTextW(IntPtr hwnd, System.Text.StringBuilder text, int count);
  [DllImport("user32.dll",SetLastError=true)] public static extern IntPtr SendMessageTimeoutW(IntPtr hwnd,uint message,IntPtr w,IntPtr l,uint flags,uint timeout,out IntPtr result);
  public static IntPtr MainWindow(uint pid) {
    IntPtr best=IntPtr.Zero; long largest=0;
    EnumWindows((hwnd,_)=> {
      GetWindowThreadProcessId(hwnd,out uint owner);
      if(owner==pid && IsWindowVisible(hwnd) && GetWindowRect(hwnd,out Rect r)) {
        long area=(long)(r.Right-r.Left)*(r.Bottom-r.Top);
        if(r.Right-r.Left>300 && r.Bottom-r.Top>200 && area>largest) {largest=area;best=hwnd;}
      }
      return true;
    },IntPtr.Zero);
    return best;
  }
  public static string Title(IntPtr hwnd) {var title=new System.Text.StringBuilder(512);GetWindowTextW(hwnd,title,512);return title.ToString();}
}
'@
    Add-Type -AssemblyName System.Drawing
    # Process.MainWindowHandle can select the intentionally visible tiny activity bar.
    # Verify the actual large app window independently.
    $handle=[WindowCapture]::MainWindow($process.Id)
    if($handle -eq [IntPtr]::Zero){throw 'No full Far Flow app window was found'}
    $reply=[IntPtr]::Zero
    if([WindowCapture]::SendMessageTimeoutW($handle,0,[IntPtr]::Zero,[IntPtr]::Zero,2,2000,[ref]$reply) -eq [IntPtr]::Zero){throw 'App window is not responding'}
    [WindowCapture]::SetForegroundWindow($handle) | Out-Null
    Start-Sleep -Seconds 1
    $rect = [WindowCapture]::VisibleBounds($handle)
    $width=$rect.Right-$rect.Left; $height=$rect.Bottom-$rect.Top
    if ($width -lt 100 -or $height -lt 100) { throw 'Window dimensions are invalid' }
    $bitmap=New-Object System.Drawing.Bitmap($width,$height)
    $graphics=[System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.CopyFromScreen($rect.Left,$rect.Top,0,0,$bitmap.Size)
        $bitmap.Save((Join-Path (Resolve-Path $EvidenceDirectory) 'window.png'))
    } finally { $graphics.Dispose(); $bitmap.Dispose() }
    if($VerifyBrand){
        if(-not(Test-Path $assetsPath)){throw 'The app did not confirm that both branded images rendered'}
        $assets=Get-Content $assetsPath | ConvertFrom-Json
        if(-not $assets.heroLoaded -or -not $assets.logoLoaded){throw 'Branded image rendering failed'}
        Copy-Item $assetsPath (Join-Path $EvidenceDirectory 'ui-assets.json')
    }
    @{
        utc=[DateTime]::UtcNow.ToString('O'); title=[WindowCapture]::Title($handle);
        responsive=$true; hwnd=$handle.ToInt64();
        width=$width; height=$height; sourceCommit=$env:GITHUB_SHA;
        environment='GitHub hosted Windows runner; no physical microphone or keyboard certification'
    } | ConvertTo-Json | Set-Content (Join-Path $EvidenceDirectory 'window-evidence.json')
    [WindowCapture]::SendMessageTimeoutW($handle,0x10,[IntPtr]::Zero,[IntPtr]::Zero,2,2000,[ref]$reply) | Out-Null
    if(-not $process.WaitForExit(5000)){throw 'The app did not exit after normal window close'}
    if($WaitForModelReady -and (Get-Process -Id $modelReady.workerProcessId -ErrorAction SilentlyContinue)){throw 'The app speech worker survived window close'}
    @{normalWindowClose='PASS';sourceCommit=$env:GITHUB_SHA} | ConvertTo-Json | Set-Content (Join-Path $EvidenceDirectory 'close-evidence.json')
} catch {
    Get-WinEvent -FilterHashtable @{LogName='Application'; StartTime=[DateTime]::Now.AddMinutes(-5)} -ErrorAction SilentlyContinue |
        Where-Object { $_.Level -le 2 } | Select-Object -First 12 TimeCreated,ProviderName,Message |
        Format-List | Out-String | Set-Content (Join-Path $EvidenceDirectory 'startup-events.txt')
    throw
} finally {
    # Ephemeral CI cleanup. Interactive tester sessions remain open until the tester closes them.
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
