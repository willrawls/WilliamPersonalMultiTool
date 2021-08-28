using System.Collections.Generic;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Tests
{
    public class TestPKeys
    {
        public static readonly List<PKey> Caps123 = new List<PKey>() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D3 };
        public static readonly List<PKey> Caps124 = new List<PKey>() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D4 };
        public static readonly List<PKey> Caps125 = new List<PKey>() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.D5 };
        public static readonly List<PKey> Caps12P = new List<PKey>() { PKey.CapsLock, PKey.D1, PKey.D2, PKey.P };
        public static readonly List<PKey> Caps1A3 = new List<PKey>() { PKey.CapsLock, PKey.D1, PKey.A, PKey.D3 };

        public static readonly List<PKey> ShiftX2 = new List<PKey>() { PKey.Shift, PKey.X };

    }
}