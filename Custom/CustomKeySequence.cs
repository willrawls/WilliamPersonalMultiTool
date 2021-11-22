using System;
using System.Collections.Generic;
using System.Drawing;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using WilliamPersonalMultiTool.Acting;

namespace WilliamPersonalMultiTool.Custom
{
    public class CustomKeySequence : KeySequence
    {
        public int BackspaceCount { get; set; }
        public string Arguments { get; set; }
        public string ExecutablePath { get; set; }
        public Color BackColor { get; set; } = Color.White;
        public List<CustomKeySequenceChoice> Choices { get; set; }
        public BaseActor Actor { get; set; }

        public CustomKeySequence(string name, List<PKey> keys, EventHandler<PhraseEventArguments> hotPhraseEventArgs, 
            int backspaceCount = 0, Color? backColor = null)
            : base(name, keys, hotPhraseEventArgs)
        {
            if (backspaceCount < 0)
                backspaceCount = 0;
            BackspaceCount = backspaceCount;
            BackColor = backColor ?? Color.White;
        }

        public CustomKeySequence(string keyText, string arguments, BaseActor actor, List<PKey> keysToPrepend = null)
        {
            Actor = actor;
            Arguments = arguments;
            Name = arguments;
            Sequence = keyText.ToPKeyList(null, out var wildcardMatchType, out var wildcardCount);
            ThenCall((sender, eventArguments) => { actor.OnAct(eventArguments); });

            if (keysToPrepend.IsEmpty())
                return;
            Sequence.InsertRange(0, keysToPrepend);
        }
    }
}