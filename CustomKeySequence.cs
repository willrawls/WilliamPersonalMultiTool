using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using Win32Interop.Enums;

namespace WilliamPersonalMultiTool
{
    public class CustomKeySequence : KeySequence
    {
        public int BackspaceCount { get; set; }
        public string Arguments { get; set; }
        public string ExecutablePath { get; set; }
        public Color BackColor { get; set; } = Color.White;
        public List<CustomKeySequenceChoice> Choices { get; set; }
        public IAct Actor { get; set; }

        public CustomKeySequence(string name, List<PKey> keys, EventHandler<PhraseEventArguments> hotPhraseEventArgs, int backspaceCount = 0, Color? backColor = null) 
            : base(name, keys, hotPhraseEventArgs)
        {
            if (backspaceCount < 0)
                backspaceCount = 0;
            BackspaceCount = backspaceCount;
            BackColor = backColor ?? Color.White;
        }
    }
}