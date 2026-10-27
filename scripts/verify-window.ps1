param(
    [Parameter(Mandatory=$true)][string]$ExePath,
    [Parameter(Mandatory=$true)][string]$EvidenceDirectory,
    [string]$Arguments = ''
)
$ErrorActionPreference = 'Stop'
New-Item $EvidenceDirectory -ItemType Directory -Force | Out-Null
$resolvedExe = (Resolve-Path $ExePath).Path
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
    Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class WindowCapture {
  [StructLayout(LayoutKind.Sequential)] public struct Rect { public int Left, Top, Right, Bottom; }
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hwnd, out Rect rect);
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
    $rect = New-Object WindowCapture+Rect
    [WindowCapture]::GetWindowRect($handle,[ref]$rect) | Out-Null
    $width=$rect.Right-$rect.Left; $height=$rect.Bottom-$rect.Top
    if ($width -lt 100 -or $height -lt 100) { throw 'Window dimensions are invalid' }
    $bitmap=New-Object System.Drawing.Bitmap($width,$height)
    $graphics=[System.Drawing.Graphics]::FromImage($bitmap)
    try {
        $graphics.CopyFromScreen($rect.Left,$rect.Top,0,0,$bitmap.Size)
        $bitmap.Save((Join-Path (Resolve-Path $EvidenceDirectory) 'window.png'))
    } finally { $graphics.Dispose(); $bitmap.Dispose() }
    @{
        utc=[DateTime]::UtcNow.ToString('O'); title=[WindowCapture]::Title($handle);
        responsive=$true; hwnd=$handle.ToInt64();
        width=$width; height=$height; sourceCommit=$env:GITHUB_SHA;
        environment='GitHub hosted Windows runner; no physical microphone or keyboard certification'
    } | ConvertTo-Json | Set-Content (Join-Path $EvidenceDirectory 'window-evidence.json')
} catch {
    Get-WinEvent -FilterHashtable @{LogName='Application'; StartTime=[DateTime]::Now.AddMinutes(-5)} -ErrorAction SilentlyContinue |
        Where-Object { $_.Level -le 2 } | Select-Object -First 12 TimeCreated,ProviderName,Message |
        Format-List | Out-String | Set-Content (Join-Path $EvidenceDirectory 'startup-events.txt')
    throw
} finally {
    # Ephemeral CI cleanup. Interactive tester sessions remain open until the tester closes them.
    if (-not $process.HasExited) { Stop-Process -Id $process.Id -Force }
}
