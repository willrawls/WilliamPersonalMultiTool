using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class TypeActor : BaseActor
    {
        public TypeActor()
        {
        }

        public BaseActor InitializeActor(string item)
        {
            if (item.IsEmpty()) return this;

            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            Verbs = new List<string>();
            ExtractVerbs(tokens, "expand", Verbs, out string arguments);

            if (Verb != "expand" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {Arguments}");

            KeySequence.Name = Arguments.Trim();
            return this;
        }

        public string TextToPaste()
        {
            var text = Clipboard.GetText();
            return text;
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}