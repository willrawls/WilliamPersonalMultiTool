using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class AdjustActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public AdjustActor(string item) : base(ActionableType.Adjust, item)
        {
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}