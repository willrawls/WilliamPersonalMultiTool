using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class RepeatActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public RepeatActor(string item)
        {
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}