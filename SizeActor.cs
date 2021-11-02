using System;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class SizeActor : BaseActor
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public SizeActor(string item) : base(ActionType.Size, item)
        {
            
            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count == 3)
            {
                Verb = tokens[0].Replace("%", "percent");
                Width = tokens[1].AsInteger();
                Height = tokens[2].AsInteger();
            }
            else if (tokens.Count > 0) // Default is "to"
            {
                Verb = "by";
                if(tokens.Count > 2) Width = tokens[0].AsInteger();
                if(tokens.Count > 3) Height = tokens[1].AsInteger();
            }

            if (Verb != "by" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {Arguments}");
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}