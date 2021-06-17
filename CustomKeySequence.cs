using System;
using System.Collections.Generic;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class CustomKeySequence : KeySequence
    {
        public int BackspaceCount { get; set; }
        public string Arguments { get; set; }
        public string ExecutablePath { get; set; }

        public CustomKeySequence(string name, List<PKey> keys, EventHandler<PhraseEventArguments> hotPhraseEventArgs, int backspaceCount) : base(name, keys, hotPhraseEventArgs)
        {
            if (backspaceCount < 0)
                backspaceCount = 0;
            BackspaceCount = backspaceCount;
        }
    }
}