using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class RunActor : BaseActor
    {
        public string TargetExecutable { get; set; }
        public string Arguments { get; set; }

        public RunActor(string item) : base(ActionableType.Run, item)
        {
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }
    }
}