using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Metadata;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class BaseActor : IAct
    {
        public BaseActor()
        {
        }

        public virtual void Initialize(string item, List<PKey> keysToPrepend)
        {
            var cleanItem = item
                .Replace("\r", "")
                .FirstToken("\n")
                .Trim();
            Separator = $" {ActionableType}";

            Arguments = cleanItem.TokensAfterFirst(Separator);
            if (Arguments.StartsWith(" "))
                Arguments = Arguments.Substring(1);

            KeyText = cleanItem.FirstToken(Separator);
            if (KeyText.ToLower().StartsWith("when ")) KeyText = KeyText.TokensAfterFirst("when ");
            if (KeyText.ToLower().StartsWith("or "))   KeyText = KeyText.TokensAfterFirst("or ");

            KeySequence = new CustomKeySequence(KeyText, Arguments, this, keysToPrepend);
        }

        public string KeyText { get; set; }

        public string Arguments { get; set; }
        public string Separator { get; set; }

        public Func<bool> OnAct { get; set; }
        public Func<string, bool> Factory { get; set; }
        public ActionableType ActionableType { get; set; }
        public List<string> Verbs { get; set; }
        public KeySequence KeySequence { get; set; }

        public virtual bool Act(PhraseEventArguments phraseEventArguments)
        {
            return OnAct == null || OnAct();
        }
    }
}