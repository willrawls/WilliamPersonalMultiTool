using System.Collections.Generic;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class AdjustActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public AdjustActor() 
        {
            ActionableType = ActionableType.Adjust;
            Verbs = new List<string>
            {
                "percent",
                "default"
            };
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}