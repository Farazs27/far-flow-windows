using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FarFlow_Windows.Services;

public sealed record TextTarget(IntPtr Window,IntPtr Focus,uint ProcessId,bool IsNotepad);

public static class TextDelivery
{
    public static TextTarget Snapshot()
    {
        var window=GetForegroundWindow();
        uint thread=GetWindowThreadProcessId(window,out uint process);
        var info=new GuiInfo { Size=Marshal.SizeOf<GuiInfo>() };
        bool readable=GetGUIThreadInfo(thread,ref info);
        bool notepad=false;
        try { using var p=Process.GetProcessById((int)process); notepad=p.ProcessName.Equals("Notepad",StringComparison.OrdinalIgnoreCase); }
        catch (Exception) { }
        return new(window,readable?info.Focus:IntPtr.Zero,process,notepad && readable && info.Focus!=IntPtr.Zero);
    }
    // Phase 1 deliberately supports Notepad only. All other targets retain text for Copy.
    public static string InsertIntoNotepad(TextTarget target,string text)
    {
        if (!target.IsNotepad) return "Ready to copy · direct insertion currently supports Notepad";
        var current=Snapshot();
        if (current.Window!=target.Window || current.Focus!=target.Focus || current.ProcessId!=target.ProcessId || !current.IsNotepad)
            return "Focus changed · text kept here for Copy";
        if (text.Length>8000) return "Long transcript · use Copy";
        if (new[]{0x10,0x11,0x12,0x5B,0x5C}.Any(k => (GetAsyncKeyState(k)&0x8000)!=0))
            return "A modifier key is held · text kept here for Copy";
        var input=new List<Input>();
        foreach(char c in text.Replace("\r\n","\n").Replace('\n','\r'))
        {
            input.Add(new Input { Type=1, Data=new Union { Key=new Keyboard { Scan=c,Flags=4 } } });
            input.Add(new Input { Type=1, Data=new Union { Key=new Keyboard { Scan=c,Flags=6 } } });
        }
        // Foreground routing is not atomic with the checks. No retry or refocusing on ambiguity.
        uint sent=SendInput((uint)input.Count,input.ToArray(),Marshal.SizeOf<Input>());
        return sent==input.Count ? "Sent to Notepad · verify at your cursor" : "Insertion was incomplete or blocked · check Notepad before using Copy";
    }
    [StructLayout(LayoutKind.Sequential)] private struct GuiInfo { public int Size; public uint Flags; public IntPtr Active,Focus,Capture,Menu,Move,Caret; public int L,T,R,B; }
    [StructLayout(LayoutKind.Sequential)] private struct Keyboard { public ushort Virtual,Scan; public uint Flags,Time; public nuint Extra; }
    [StructLayout(LayoutKind.Sequential)] private struct Mouse { public int X,Y; public uint Data,Flags,Time; public nuint Extra; }
    [StructLayout(LayoutKind.Explicit)] private struct Union { [FieldOffset(0)] public Keyboard Key; [FieldOffset(0)] public Mouse Mouse; }
    [StructLayout(LayoutKind.Sequential)] private struct Input { public uint Type; public Union Data; }
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window,out uint process);
    [DllImport("user32.dll")] private static extern bool GetGUIThreadInfo(uint thread,ref GuiInfo info);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    [DllImport("user32.dll",SetLastError=true)] private static extern uint SendInput(uint count,Input[] input,int size);
}
