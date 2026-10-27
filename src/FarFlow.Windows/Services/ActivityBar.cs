using System.Runtime.InteropServices;

namespace FarFlow_Windows.Services;

// Small native nonactivating overlay; it never takes the caret from the dictation target.
public sealed class ActivityBar : IDisposable
{
    private readonly WindowProcedure procedure;
    private readonly string className="FarFlowBar-"+Guid.NewGuid().ToString("N");
    private IntPtr window;
    private bool listening;
    private float level;
    public ActivityBar()
    {
        procedure=Handle;
        var cls=new WindowClass { Size=(uint)Marshal.SizeOf<WindowClass>(),Instance=GetModuleHandleW(null),Name=className,Procedure=Marshal.GetFunctionPointerForDelegate(procedure),Cursor=LoadCursorW(IntPtr.Zero,(IntPtr)32512) };
        if(RegisterClassExW(ref cls)==0) throw new System.ComponentModel.Win32Exception();
        if(!SystemParametersInfoW(0x30,0,out var area,0)) area=new Rect { Right=1200,Bottom=800 };
        window=CreateWindowExW(0x08000088,className,"Far Flow · ready",0x80000000,
            (area.Right+area.Left)/2-72,area.Bottom-58,144,30,IntPtr.Zero,IntPtr.Zero,cls.Instance,IntPtr.Zero);
        if(window==IntPtr.Zero) throw new System.ComponentModel.Win32Exception();
        ShowWindow(window,4);
    }
    public void Update(bool recording,float amplitude=0)
    { listening=recording;level=amplitude;InvalidateRect(window,IntPtr.Zero,false); }
    private IntPtr Handle(IntPtr hwnd,uint message,IntPtr w,IntPtr l)
    {
        if(message==0x21) return (IntPtr)3; // MA_NOACTIVATE, including hover-to-activate settings.
        if(message==0x201) { ReleaseCapture();SendMessageW(hwnd,0xA1,(IntPtr)2,IntPtr.Zero);return IntPtr.Zero; }
        if(message==0x14) return (IntPtr)1;
        if(message==0xF)
        {
            IntPtr dc=BeginPaint(hwnd,out var paint);
            IntPtr background=CreateSolidBrush(0x002A2D2B),foreground=CreateSolidBrush(0x00DDE6E3);
            IntPtr oldBrush=SelectObject(dc,background),oldPen=SelectObject(dc,GetStockObject(8));
            RoundRect(dc,0,0,144,30,14,14);
            SelectObject(dc,foreground);
            if(listening)
            {
                for(int i=0;i<19;i++)
                {
                    double envelope=.35+.65*Math.Sin((i+1)*1.7+Environment.TickCount64*.014);
                    int height=3+(int)(Math.Abs(envelope)*Math.Max(.07f,level)*19);
                    RoundRect(dc,18+i*6,15-height/2,21+i*6,16+height/2,3,3);
                }
            }
            else { RoundRect(dc,53,13,91,17,4,4); }
            SelectObject(dc,oldBrush);SelectObject(dc,oldPen);DeleteObject(background);DeleteObject(foreground);
            EndPaint(hwnd,ref paint);return IntPtr.Zero;
        }
        return DefWindowProcW(hwnd,message,w,l);
    }
    public void Dispose() { if(window!=IntPtr.Zero) { DestroyWindow(window);window=IntPtr.Zero; } UnregisterClassW(className,GetModuleHandleW(null)); }
    private delegate IntPtr WindowProcedure(IntPtr hwnd,uint message,IntPtr w,IntPtr l);
    [StructLayout(LayoutKind.Sequential)] private struct Rect { public int Left,Top,Right,Bottom; }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] private struct WindowClass
    { public uint Size,Style;public IntPtr Procedure;public int ExtraClass,ExtraWindow;public IntPtr Instance,Icon,Cursor,Background;public string? Menu,Name;public IntPtr SmallIcon; }
    [StructLayout(LayoutKind.Sequential)] private struct PaintStruct
    { public IntPtr Dc;public int Erase;public Rect Area;public int Restore,Update;[MarshalAs(UnmanagedType.ByValArray,SizeConst=32)]public byte[] Reserved; }
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)]private static extern IntPtr GetModuleHandleW(string? name);
    [DllImport("user32.dll",CharSet=CharSet.Unicode,SetLastError=true)]private static extern ushort RegisterClassExW(ref WindowClass cls);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)]private static extern bool UnregisterClassW(string name,IntPtr instance);
    [DllImport("user32.dll",CharSet=CharSet.Unicode,SetLastError=true)]private static extern IntPtr CreateWindowExW(uint ex,string cls,string title,uint style,int x,int y,int width,int height,IntPtr parent,IntPtr menu,IntPtr instance,IntPtr param);
    [DllImport("user32.dll")]private static extern bool ShowWindow(IntPtr hwnd,int mode);
    [DllImport("user32.dll")]private static extern bool DestroyWindow(IntPtr hwnd);
    [DllImport("user32.dll")]private static extern bool InvalidateRect(IntPtr hwnd,IntPtr rect,bool erase);
    [DllImport("user32.dll")]private static extern IntPtr BeginPaint(IntPtr hwnd,out PaintStruct paint);
    [DllImport("user32.dll")]private static extern bool EndPaint(IntPtr hwnd,ref PaintStruct paint);
    [DllImport("user32.dll")]private static extern IntPtr DefWindowProcW(IntPtr hwnd,uint message,IntPtr w,IntPtr l);
    [DllImport("user32.dll")]private static extern bool ReleaseCapture();
    [DllImport("user32.dll")]private static extern IntPtr SendMessageW(IntPtr hwnd,uint message,IntPtr w,IntPtr l);
    [DllImport("user32.dll")]private static extern IntPtr LoadCursorW(IntPtr instance,IntPtr name);
    [DllImport("user32.dll")]private static extern bool SystemParametersInfoW(uint action,uint param,out Rect rect,uint flags);
    [DllImport("gdi32.dll")]private static extern IntPtr CreateSolidBrush(uint color);
    [DllImport("gdi32.dll")]private static extern IntPtr GetStockObject(int id);
    [DllImport("gdi32.dll")]private static extern IntPtr SelectObject(IntPtr dc,IntPtr obj);
    [DllImport("gdi32.dll")]private static extern bool DeleteObject(IntPtr obj);
    [DllImport("gdi32.dll")]private static extern bool RoundRect(IntPtr dc,int left,int top,int right,int bottom,int width,int height);
}
