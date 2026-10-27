using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FarFlow_Windows.Services;

public sealed class GlobalTrigger : IDisposable
{
    private readonly Thread thread;
    private readonly Hook callback;
    private IntPtr hook;
    private uint threadId;
    private readonly ManualResetEventSlim started = new();
    private Exception? startupError;
    public event Action<bool, long>? Edge;
    public event Action? Cancel;
    public GlobalTrigger()
    {
        callback = Handle;
        thread = new Thread(Run) { IsBackground = true, Name = "Far Flow trigger" };
        thread.Start();
        if (!started.Wait(3000)) throw new TimeoutException("The global shortcut did not start.");
        if (startupError is not null) throw startupError;
    }
    private void Run()
    {
        threadId = GetCurrentThreadId();
        PeekMessageW(out _, IntPtr.Zero, 0, 0, 0); // Establish the queue before publishing ready.
        hook = SetWindowsHookExW(13, callback, GetModuleHandleW(null), 0);
        if (hook == IntPtr.Zero) startupError = new Win32Exception();
        started.Set();
        if (startupError is not null) return;
        while (GetMessageW(out var message, IntPtr.Zero, 0, 0) > 0) { TranslateMessage(ref message); DispatchMessageW(ref message); }
        UnhookWindowsHookEx(hook); hook = IntPtr.Zero;
    }
    private IntPtr Handle(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0)
        {
            var data = Marshal.PtrToStructure<KeyData>(lParam);
            bool pressed = wParam.ToInt64() is 0x100 or 0x104;
            bool released = wParam.ToInt64() is 0x101 or 0x105;
            if ((data.Flags & 0x10) == 0 && (pressed || released))
            {
                if (data.Key == 0x77) { Edge?.Invoke(pressed, Environment.TickCount64); return (IntPtr)1; } // Timestamp before the UI queue.
                if (data.Key == 0x1B && pressed) Cancel?.Invoke();
            }
        }
        return CallNextHookEx(hook, code, wParam, lParam);
    }
    public void Dispose()
    {
        if (threadId != 0) PostThreadMessageW(threadId, 0x12, IntPtr.Zero, IntPtr.Zero);
        if (thread != Thread.CurrentThread) thread.Join(1000);
    }
    private delegate IntPtr Hook(int code, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential)] private struct KeyData { public uint Key, Scan, Flags, Time; public nuint Extra; }
    [StructLayout(LayoutKind.Sequential)] private struct Message { public IntPtr Window; public uint Id; public nuint WParam; public nint LParam; public uint Time; public int X, Y; public uint Private; }
    [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookExW(int id, Hook callback, IntPtr module, uint thread);
    [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr handle);
    [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr w, IntPtr l);
    [DllImport("user32.dll")] private static extern int GetMessageW(out Message message, IntPtr window, uint min, uint max);
    [DllImport("user32.dll")] private static extern bool PeekMessageW(out Message message, IntPtr window, uint min, uint max, uint remove);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(ref Message message);
    [DllImport("user32.dll")] private static extern IntPtr DispatchMessageW(ref Message message);
    [DllImport("user32.dll")] private static extern bool PostThreadMessageW(uint thread, uint message, IntPtr w, IntPtr l);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandleW(string? name);
}
