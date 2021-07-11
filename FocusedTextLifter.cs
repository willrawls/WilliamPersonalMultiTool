using System;
using System.Runtime.InteropServices;
using Win32Interop.Enums;
using Win32Interop.Methods;

namespace WilliamPersonalMultiTool
{
    public class FocusedTextLifter
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([In] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        //Get the text of the focused control
        public string GetTextFromFocusedControl()
        {
            try
            {
                var activeWinPtr = User32.GetForegroundWindow();
                var activeThreadId = User32.GetWindowThreadProcessId(activeWinPtr, IntPtr.Zero);

                var currentThreadId = GetCurrentThreadId();

                if (activeThreadId != currentThreadId)
                {
                    User32.AttachThreadInput(activeThreadId, currentThreadId, true);
                }

                var activeControlId = User32.GetFocus();
                return GetText(activeControlId);
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        //Get the text of the control at the mouse position
        private string GetTextFromControlAtMousePosition()
        {
            try
            {
                if (!User32.GetCursorPos(out var p)) return "";

                var ptr = User32.WindowFromPoint(p);
                return ptr != IntPtr.Zero ? GetText(ptr) : "";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        //Get the text of a control with its handle
        private string GetText(IntPtr handle)
        {
            var maxLength = 2048;
            var buffer = Marshal.AllocHGlobal((maxLength + 1) * 2);
            SendMessageW(handle, (int) WM.WM_GETTEXT, maxLength, buffer);
            var w = Marshal.PtrToStringUni(buffer);
            Marshal.FreeHGlobal(buffer);
            return w;
        }
    }
}