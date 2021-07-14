using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MetX.Standard.Library;
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
            new RECT {left = 108, top = 29, right = 1913, bottom = 1070},   // D1
            new RECT {left = 1373, top = 730, right = 1916, bottom = 1070}, // D8
            new RECT {left = 1935, top = 337, right = 3281, bottom = 1077}, // D2
            new RECT {left = 1383, top = 9, right = 1926, bottom = 731},    // D3
            new RECT {left = 2000, top = 348, right = 3225, bottom = 1056}, // D4
            new RECT {left = 143, top = 63, right = 1779, bottom = 1016},   // D5
            new RECT {left = 1923, top = 333, right = 3180, bottom = 967},  // D6
            new RECT {left = 364, top = 99, right = 1465, bottom = 989},    // D7
            new RECT {left = 586, top = 388, right = 1325, bottom = 818},   // D9 
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

                // Copy text from foreground control Scrapped. Just didn't work
                //new("Show text from focused control", new List<PKey> {PKey.Shift, PKey.Shift, PKey.Shift, PKey.Shift}, OnShowFocusedText),
                //new("Show text from control under mouse pointer", new List<PKey> {PKey.Shift, PKey.Shift, PKey.Shift, PKey.ControlKey}, OnShowPointedAtText),
            };
        }

        /* Copy text from foreground control Scrapped. Just didn't work
        public void OnShowPointedAtText(object sender, PhraseEventArguments e)
        {
            if(Clipboard.ContainsText())
            {
                Thread.Sleep(100);
                var oldClipboardText = Clipboard.GetText();

                SendKeys.SendWait("^C");
                Thread.Sleep(100);
                var text = Clipboard.GetText();

                Clipboard.SetText(oldClipboardText);

                if(text.IsNotEmpty())
                    MessageBox.Show(text);

                MessageBox.Show("Before: " + oldClipboardText);
                MessageBox.Show("After: " + Clipboard.GetText());
            }

            /*
            var proxy = new FocusedTextLifter();
            var text = proxy.GetTextFromControlAtMousePosition();
            if(text.IsNotEmpty())
                MessageBox.Show(text);
        #1#
        }

        public void OnShowFocusedText(object sender, PhraseEventArguments e)
        {
            var proxy = new FocusedTextLifter();
            var text = proxy.GetTextFromFocusedControl();
            if(text.IsNotEmpty())
                MessageBox.Show(text);
        }
        */

        public void OnGetWindowPosition(object sender, PhraseEventArguments e)
        {
            Manager.SendBackspaces(2);

            var handle = User32.GetForegroundWindow();
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
            var triggered = e.State.KeySequence;
            var entry = triggered.Sequence[^1] - PKey.D0;

            var backspaceCount = triggered.BackspacesToSend();
            Manager.SendBackspaces(backspaceCount);

            if(entry is >= 1 and <= 9)
            {
                CurrentPosition = entry;

                var p = WindowPositions[entry - 1];
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
            var entry = CurrentPosition;

            if(entry is >= 1 and <= 9)
            {
                var p = WindowPositions[entry - 1];
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
            var handle = User32.GetForegroundWindow();

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
                    RECT rect = ShiftABit(p.Value);
                    flags = SWP.SWP_SHOWWINDOW;
                    User32.SetWindowPos(handle, IntPtr.Zero, 
                        rect.left, 
                        rect.top, 
                        rect.right - rect.left, 
                        rect.bottom - rect.top,
                        flags);
                }
            }
        }

        public int AmountOfShift = -10;
        private RECT ShiftABit(RECT rect)
        {
            AmountOfShift += 10;
            if (AmountOfShift >= 50)
                AmountOfShift = -20;

            var shifted = new RECT
            {
                left = rect.left + AmountOfShift,
                top = rect.top + AmountOfShift,
                right = rect.right, // + amount,
                bottom = rect.bottom, // + amount,
            };
            return shifted;
        }
    }
}
