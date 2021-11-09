using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class RepeatActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public RepeatActor(string item)
        {
            Last = AddLegalVerb("last");
            
            OnAct = Act;
            DefaultVerb = Last;
            CanContinue = false;
            ActionableType = ActionableType.Repeat;
        }

        public Verb Last { get; set; }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            return true;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}