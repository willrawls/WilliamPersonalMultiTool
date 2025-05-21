using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WilliamPersonalMultiTool.Custom;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace WilliamPersonalMultiTool
{
    public class WindowWorker
    {
        public static Rectangle NewRectangle(int left, int top, int right, int bottom)
        {
            return new Rectangle
            {
                X = left,
                Y = top,
                Width = right - left,
                Height = bottom - top,
                Location = new Point(left, top),
                Size = new Size(right - left, bottom - top),
            };
        }

        public static Rectangle Offset(int left, int top, int right, int bottom)
        {
            return NewRectangle(left, top, right, bottom);
        }

        public static Rectangle Offset(int offset)
        {
            return NewRectangle(offset, offset, -offset * 2, -offset * 2);
        }

        public List<Rectangle> WindowOffsets { get; set; } = new()
        {
            Offset(10),
            Offset(20),
            Offset(30),
            Offset(40),
            Offset(0),
            Offset(-10),
            Offset(-20),
            Offset(-30),
            Offset(-40),
        };

        public List<Rectangle> WindowPositions { get; set; } = new()
        {
            NewRectangle(108, 29, 1913, 1070), // D1
            NewRectangle(143, 63, 1779, 1016), // D2
            NewRectangle(364, 10, 1465, 600), // D3
            NewRectangle(364, 99, 1465, 989), // D4
            NewRectangle(586, 388, 1325, 818), // D5 
            NewRectangle(1373, 730, 1916, 1070), // D6

            NewRectangle(1383, 9, 1926, 731), // D7
            NewRectangle(1923, 333, 3180, 967), // D8
            NewRectangle(1935, 337, 3281, 1077), // D9
            NewRectangle(2000, 348, 3225, 1056), // D0
        };

        public int CurrentPosition = 0;

        public int CurrentScreen = 0;
        public int CurrentCorner = 0;

        public CustomPhraseManager Manager { get; }
        public static IntPtr ParentHandle { get; private set; }
        public List<CustomKeySequence> Sequences { get; set; }

        public WindowWorker(CustomPhraseManager Manager, IntPtr parentHandle)
        {
            this.Manager = Manager;
            ParentHandle = parentHandle;
            Sequences = new List<CustomKeySequence>
            {
                new("Window to position 1", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D1 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 2", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D2 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 3", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D3 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 4", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D4 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 5", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D5 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 6", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D6 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 7", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D7 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 8", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D8 },
                    OnMoveCurrentWindowToPosition),
                new("Window to position 9", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.ControlKey, PKey.D9 },
                    OnMoveCurrentWindowToPosition),

                new("Window to upper left", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D1 },
                    OnMoveCurrentWindowToCorner),
                new("Window to upper right", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D2 },
                    OnMoveCurrentWindowToCorner),
                new("Window to lower right", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D3 },
                    OnMoveCurrentWindowToCorner),
                new("Window to lower left", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D4 },
                    OnMoveCurrentWindowToCorner),

                new("Window to screen 2 upper left", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D7 },
                    OnMoveCurrentWindowToCorner),
                new("Window to screen 2 upper right", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D8 },
                    OnMoveCurrentWindowToCorner),
                new("Window to screen 2 lower right", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D9 },
                    OnMoveCurrentWindowToCorner),
                new("Window to screen 2 lower left", new List<PKey> { PKey.ControlKey, PKey.Shift, PKey.D9 },
                    OnMoveCurrentWindowToCorner),

                new("Window to next position", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.RShiftKey },
                    OnMoveCurrentWindowToNextPosition),
                new("Window to previous position", new List<PKey> { PKey.CapsLock, PKey.CapsLock, PKey.LShiftKey },
                    OnMoveCurrentWindowToPreviousPosition),

                new("Get window position", new List<PKey> { PKey.ControlKey, PKey.ControlKey, PKey.G, PKey.W },
                    OnGetWindowPosition),
            };
            Sequences.ForEach(s =>
            {
                s.BackColor = Color.Aqua;
                s.ForeColor = Color.Black;
            });
        }

        public Rectangle CalculateCornerForSystem(Rectangle rect, int screen, int corner)
        {
            Rectangle screenBounds;
            if (screen == 0)
            {
                screenBounds = Screen.PrimaryScreen.WorkingArea;
            }
            else
            {
                screenBounds = Screen.AllScreens[^1].WorkingArea;
            }

            var originalWindowPosition = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var newWindowPosition = originalWindowPosition;

            if (corner > 3) corner = 0;
            if (corner < 0) corner = 3;

            //                          left, top
            switch (corner)
            {
                // 0: upper left corner is 0, 0 offset from screenBounds, width and height the same
                case 1:
                    newWindowPosition =
                        new Rectangle(0, 0, originalWindowPosition.Width, originalWindowPosition.Height);
                    break;

                // 1: upper right corner is screen.right-window.width, 0
                case 2:
                    newWindowPosition = new Rectangle(screenBounds.Right - originalWindowPosition.Width, 0,
                        originalWindowPosition.Width, originalWindowPosition.Height);
                    break;

                // 2: lower left corner is 0, screen.bottom-window.height offset from screenBounds
                case 3:
                    newWindowPosition = new Rectangle(0, screenBounds.Bottom - originalWindowPosition.Height,
                        originalWindowPosition.Width, originalWindowPosition.Height);
                    break;

                // 3: lower right corner is screen.right-window.width, screen.bottom-window.height
                case 0:
                    newWindowPosition = new Rectangle(screenBounds.Right - originalWindowPosition.Width,
                        screenBounds.Bottom - originalWindowPosition.Height, originalWindowPosition.Width,
                        originalWindowPosition.Height);
                    break;
            }

            var cornerSystem = NewRectangle(newWindowPosition);
            return cornerSystem;
        }

        private Rectangle NewRectangle(Rectangle other)
        {
            return NewRectangle(other.Left, other.Top, other.Right, other.Bottom);
        }

        public void OnGetWindowPosition(object sender, PhraseEventArguments e)
        {
            Manager.SendBackspaces(2);


            var handle = PInvoke.GetForegroundWindow();
            if (handle != HWND.Null)
            {
                WINDOWINFO info = new();
                info.cbSize = (uint)Marshal.SizeOf(info);
                PInvoke.GetWindowInfo(handle, ref info);

                var answer =
                    $"            new System.Drawing.Rectangle {{left = {info.rcWindow.left}, top = {info.rcWindow.top}, right = {info.rcWindow.right}, bottom = {info.rcWindow.bottom}}},";
                Clipboard.SetText(answer);
            }
        }

        public void OnMoveCurrentWindowToCorner(object sender, PhraseEventArguments e)
        {
            var triggered = e.State.KeySequence;
            var entry = triggered.Sequence[^1] - PKey.D0;

            var backspaceCount = triggered.BackspacesToSend();
            Manager.SendBackspaces(backspaceCount);

            if (entry is >= 0 and <= 8)
            {
                var handle = PInvoke.GetForegroundWindow();
                if (handle == ParentHandle)
                    return;

                if (handle == HWND.Null) return;

                PInvoke.GetWindowRect(handle, out var startingWindowPosition);
                var newPosition = CalculateCornerForSystem(startingWindowPosition, 0, entry);
                MoveForegroundWindowTo(newPosition);
            }
        }

        public void OnMoveCurrentWindowToPosition(object sender, PhraseEventArguments e)
        {
            var triggered = e.State.KeySequence;
            var entry = triggered.Sequence[^1] - PKey.D0;

            var backspaceCount = triggered.BackspacesToSend();
            Manager.SendBackspaces(backspaceCount);

            if (entry is >= 0 and <= 9)
            {
                CurrentPosition = entry;

                var p = WindowPositions[entry];
                MoveForegroundWindowTo(p);
            }
        }

        public void OnMoveCurrentWindowToPreviousPosition(object sender, PhraseEventArguments e)
        {
            CurrentPosition--;
            if (CurrentPosition < 0)
            {
                CurrentPosition = 9;
            }

            var entry = CurrentPosition;

            if (entry is < 1 or > 9) return;

            var p = WindowPositions[entry - 1];
            MoveForegroundWindowTo(p);
        }

        public void OnMoveCurrentWindowToNextPosition(object sender, PhraseEventArguments e)
        {
            CurrentPosition++;
            if (CurrentPosition > 9)
            {
                CurrentPosition = 0;
            }

            var entry = CurrentPosition;

            if (entry is < 1 or > 9) return;

            var p = WindowPositions[entry - 1];
            MoveForegroundWindowTo(p);
        }

        public void OnMoveTo00(object sender, PhraseEventArguments e)
        {
            Manager.SendBackspaces(2);
            MoveForegroundWindowTo(null);
        }

        public static Rectangle? GetForegroundWindowPosition()
        {
            var handle = PInvoke.GetForegroundWindow();

            if (handle == HWND.Null)
                return null;

            return PInvoke.GetWindowRect(handle, out var position)
                ? position
                : null;
        }

        public static void MoveForegroundWindowTo(Rectangle? p)
        {
            var handle = PInvoke.GetForegroundWindow();

            if (handle == ParentHandle)
                return;

            if (handle != HWND.Null)
            {
                SET_WINDOW_POS_FLAGS flags;

                if (p == null)
                {
                    flags = SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOSIZE;
                    PInvoke.SetWindowPos(handle, HWND.Null, 50, 50, 0, 0, flags);
                }
                else
                {
                    var rect = ShiftABit(p.Value);
                    flags = SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW;
                    PInvoke.SetWindowPos(handle, HWND.Null,
                        rect.X,
                        rect.Y,
                        rect.Width,
                        rect.Height,
                        flags);
                }
            }
        }

        public static int AmountOfShift = -10;

        private static Rectangle ShiftABit(Rectangle rect)
        {
            AmountOfShift += 10;
            if (AmountOfShift >= 50)
                AmountOfShift = -20;

            var shifted = NewRectangle(rect.Left + AmountOfShift, rect.Top, rect.Right, rect.Bottom);
            return shifted;
        }
    }
}