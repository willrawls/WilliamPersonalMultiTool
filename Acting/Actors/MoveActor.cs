using System;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class MoveActor : BaseActor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Verb Relative { get; set; }
        public Verb Percent { get; set; }
        public Verb To { get; set; }

        public MoveActor()
        {
            ActionableType = ActionableType.Move;

            // Move window to a specific location
            To = AddLegalVerb("to");                         
            
            // Move window by a certain amount
            Percent = AddLegalVerb("percent", To);            

            // With relative coordinates
            Relative = AddLegalVerb("relative");

            OnAct = Act;
        }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count != 4)
            {
                Errors = "Move actor: wrong number of arguments";
                return false;
            }

            Left = tokens[0].AsInteger();
            Top = tokens[1].AsInteger();
            Width = tokens[2].AsInteger();
            Height = tokens[3].AsInteger();

            return true;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            if (Errors.IsNotEmpty())
                return true;

            return true;
        }
    }
}