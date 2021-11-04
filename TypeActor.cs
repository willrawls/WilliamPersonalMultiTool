using System;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public interface IInitializeActor
    {
        void InitializeActor(string item);
    }

    public class TypeActor : BaseActor
    {
        public TypeActor()
        {
        }

        public sealed override void InitializeActor(string item)
        {
            InitializeBase(ActionableType.Type, item, null);
            if (item.IsEmpty()) return;

            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            Verb = tokens.Count > 1
                ? tokens[0]
                : "expand";

            if (Verb != "expand" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {Arguments}");

            KeySequence.Name = Arguments.Trim();
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