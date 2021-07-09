using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using User32 = Win32Interop.Methods.User32;
using Win32Interop.Enums;
using Win32Interop.Structs;

namespace WilliamPersonalMultiTool
{
    public class WindowWorker
    {
        private List<RECT> WindowPositions { get; set; } = new()
        {
            new RECT {left = 143, top = 63, right = 1779, bottom = 1016},   // D1
            new RECT {left = 2000, top = 348, right = 3225, bottom = 1056},
            new RECT {left = 586, top = 388, right = 1325, bottom = 818},
            new RECT {left = 1923, top = 333, right = 3180, bottom = 967},
            new RECT {left = 1373, top = 730, right = 1916, bottom = 1070},
            new RECT {left = 1383, top = 9, right = 1926, bottom = 731},
            new RECT {left = 364, top = 99, right = 1465, bottom = 989},
            new RECT {left = 108, top = 29, right = 1913, bottom = 1070},
            new RECT {left = 1935, top = 337, right = 3281, bottom = 1077}, // D9

        };

        public int CurrentPosition = 0;
        public CustomPhraseManager Manager { get; }
        public IntPtr ParentHandle { get; private set; }
        public List<CustomKeySequence> Sequences { get; set; }

        public WindowWorker(CustomPhraseManager Manager, IntPtr parentHandle)
        {
            this.Manager = Manager;
            ParentHandle = parentHandle;
            Sequences = new List<CustomKeySequence>
            {
                new("Move window to position 1", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D1}, OnMoveCurrentWindowToPosition),
                new("Move window to position 2", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D2}, OnMoveCurrentWindowToPosition),
                new("Move window to position 3", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D3}, OnMoveCurrentWindowToPosition),
                new("Move window to position 4", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D4}, OnMoveCurrentWindowToPosition),
                new("Move window to position 5", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D5}, OnMoveCurrentWindowToPosition),
                new("Move window to position 6", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D6}, OnMoveCurrentWindowToPosition),
                new("Move window to position 7", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D7}, OnMoveCurrentWindowToPosition),
                new("Move window to position 8", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D8}, OnMoveCurrentWindowToPosition),
                new("Move window to position 9", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.D9}, OnMoveCurrentWindowToPosition),

                new("Move window to next position", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.ControlKey}, OnMoveCurrentWindowToNextPosition),

                new("Get window position", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.G, PKey.W}, OnGetWindowPosition),

                new("Move window to 0,0", new List<PKey> {PKey.ControlKey, PKey.ControlKey, PKey.Shift, PKey.D0}, OnMoveTo00),
            };
        }

        public void OnGetWindowPosition(object sender, PhraseEventArguments e)
        {
            Manager.SendBackspaces(2);

            IntPtr handle = User32.GetForegroundWindow();
            if (handle != IntPtr.Zero)
            {
                WINDOWINFO info = new();
                info.cbSize = (uint) Marshal.SizeOf(info);
                User32.GetWindowInfo(handle, ref info);

                var answer = $"            new RECT {{left = {info.rcWindow.left}, top = {info.rcWindow.top}, right = {info.rcWindow.right}, bottom = {info.rcWindow.bottom}}},";
                Clipboard.SetText(answer);
            }
        }

        public void OnMoveCurrentWindowToPosition(object sender, PhraseEventArguments e){
            int entry = e.State.KeySequence.Sequence[^1] - PKey.D0;
            Manager.SendBackspaces(2);

            if(entry is >= 1 and <= 9)
            {
                CurrentPosition = entry;

                RECT p = WindowPositions[entry - 1];
                MoveTo(p);
            }

        }

        public void OnMoveCurrentWindowToNextPosition(object sender, PhraseEventArguments e)
        {
            CurrentPosition++;
            if (CurrentPosition > 9)
            {
                CurrentPosition = 0;
            }
            int entry = CurrentPosition;

            if(entry is >= 1 and <= 9)
            {
                RECT p = WindowPositions[entry - 1];
                MoveTo(p);
            }
        }

        public void OnMoveTo00(object sender, PhraseEventArguments e)
        { 
            Manager.SendBackspaces(2);
            MoveTo(null);
        }

        private void MoveTo(RECT? p)
        {
            IntPtr handle = User32.GetForegroundWindow();

            if (handle == ParentHandle)
                return;

            if (handle != IntPtr.Zero)
            {
                SWP flags;

                if (p == null)
                {
                    flags = SWP.SWP_SHOWWINDOW | SWP.SWP_NOSIZE;
                    User32.SetWindowPos(handle, IntPtr.Zero, 50, 50, 0, 0, flags);
                }
                else
                {
                    flags = SWP.SWP_SHOWWINDOW;
                    User32.SetWindowPos(handle, IntPtr.Zero, 
                        p.Value.left, 
                        p.Value.top, 
                        p.Value.right - p.Value.left, 
                        p.Value.bottom - p.Value.top,
                        flags);
                }
            }
        }
    }
}
