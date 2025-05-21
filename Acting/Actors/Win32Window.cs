using System;
using System.Windows.Forms;
using Windows.Win32;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class Win32Window : IWin32Window
    {
        public static IWin32Window ActiveWindow
        {
            get { return new Win32Window(PInvoke.GetForegroundWindow()); }
        }

        public Win32Window(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }
    }
}