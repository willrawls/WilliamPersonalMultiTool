using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class ChooseActor : BaseActor
    {
        public ChooseActor(string item) : base(ActionableType.Choose, item)
        {
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}