using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using FarFlow_Windows.Services;

internal static class NotepadSmoke
{
    public static async Task RunAsync()
    {
        using var editor=Process.Start(new ProcessStartInfo("notepad.exe"){UseShellExecute=false}) ?? throw new Exception("Notepad could not start");
        try
        {
            var deadline=DateTime.UtcNow.AddSeconds(20);
            do{await Task.Delay(100);editor.Refresh();}while(editor.MainWindowHandle==IntPtr.Zero&&!editor.HasExited&&DateTime.UtcNow<deadline);
            if(editor.HasExited||editor.MainWindowHandle==IntPtr.Zero)throw new Exception("No test Notepad window");
            SetForegroundWindow(editor.MainWindowHandle);await Task.Delay(300);
            var target=TextDelivery.Snapshot();
            if(!target.IsNotepad||target.ProcessId!=editor.Id)throw new Exception("Test Notepad is not the foreground target");
            const string fixture="Far Flow test — één twee three.";
            var delivery=TextDelivery.InsertIntoNotepad(target,fixture);
            await Task.Delay(400);
            var text=ReadText(target.Focus);
            if(text!=fixture)throw new Exception("Notepad Unicode insertion mismatch: "+delivery);
            keybd_event(0x11,0,0,0);keybd_event(0x46,0,0,0);keybd_event(0x46,0,2,0);keybd_event(0x11,0,2,0);
            await Task.Delay(400);
            var changed=TextDelivery.Snapshot();
            if(changed.Window==target.Window&&changed.Focus==target.Focus)throw new Exception("Test focus did not change");
            var rejected=TextDelivery.InsertIntoNotepad(target,"SHOULD NOT APPEAR");
            if(!rejected.StartsWith("Focus changed",StringComparison.Ordinal)||ReadText(target.Focus)!=fixture)throw new Exception("Changed-target delivery was not safely rejected");
            Directory.CreateDirectory("artifacts/evidence");
            await File.WriteAllTextAsync("artifacts/evidence/notepad.json",JsonSerializer.Serialize(new{unicodeRoundTrip=true,changedFocusRejected=true,environment="GitHub Windows runner Notepad; modern Win11 editor and physical microphone remain tester gates",sourceCommit=Environment.GetEnvironmentVariable("GITHUB_SHA")}));
            Console.WriteLine("Notepad Unicode insertion and changed-focus recovery passed.");
        }
        finally{if(!editor.HasExited)editor.Kill();}
    }
    private static string ReadText(IntPtr window)
    {
        var text=new StringBuilder(4096);
        if(SendMessageTimeoutW(window,0xD,(nuint)text.Capacity,text,2,1000,out _)==IntPtr.Zero)throw new Exception("Test editor did not return text");
        return text.ToString();
    }
    [DllImport("user32.dll")]private static extern bool SetForegroundWindow(IntPtr hwnd);
    [DllImport("user32.dll")]private static extern void keybd_event(byte key,byte scan,uint flags,nuint extra);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)]private static extern IntPtr SendMessageTimeoutW(IntPtr hwnd,uint message,nuint w,StringBuilder text,uint flags,uint timeout,out nuint result);
}
