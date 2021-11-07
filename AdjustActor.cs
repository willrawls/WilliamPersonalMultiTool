using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class RandomActor : BaseActor
    {
        public int Count { get; set; } = 10;
        public int Sides { get; set; } = 6;
        public string Before { get; set; } = "";
        public string After { get; set; } = "";

        public Verb Dice { get; set; }
        public Verb Number { get; set; }
        public Verb Digits { get; set; }
        public Verb Letters { get; set; }


        public RandomActor()
        {
            Letters = AddLegalVerb("letters");
            Digits = AddLegalVerb("digits");
            Number = AddLegalVerb("number");
            Dice = AddLegalVerb("dice");

            OnAct = Act;
            DefaultVerb = Number;
            CanContinue = false;
        }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            << Start here
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            return false;
        }
    }
}