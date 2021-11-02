using System;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class MoveActor : BaseActor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public MoveActor(string item) : base(ActionType.Move, item)
        {
            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count == 5)
            {
                Verb = tokens[0].Replace("%", "percent");
                Left = tokens[1].AsInteger();
                Top = tokens[2].AsInteger();
                Width = tokens[3].AsInteger();
                Height = tokens[4].AsInteger();
            }
            else if (tokens.Count > 0) // Default is "to"
            {
                Verb = "to";
                if(tokens.Count > 0) Left = tokens[0].AsInteger();
                if(tokens.Count > 1) Top = tokens[1].AsInteger();
                if(tokens.Count > 2) Width = tokens[2].AsInteger();
                if(tokens.Count > 3) Height = tokens[3].AsInteger();
            }

            if (Verb != "to" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {Arguments}");
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            if (Verb.IsEmpty())
                return false;

            switch (Verb.ToLower())
            {
                case "to":
                    break;

                case "%":
                case "percent":
                    break;

                default:
                    break;
            }
            return true;
        }
    }
}