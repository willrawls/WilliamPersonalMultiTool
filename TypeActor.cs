using System;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class TypeActor : BaseActor
    {
        public TypeActor(string item) : base(ActionableType.Type, item)
        {
            if (item.IsEmpty()) return;
            <<< Start here:
            //      All the actors need Initialize or something equivalent
            //      
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