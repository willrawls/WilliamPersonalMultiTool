﻿using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;
using WilliamPersonalMultiTool.Custom;

namespace WilliamPersonalMultiTool.Acting.Actors
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

            var counts = Arguments.FirstToken("\"");
            
            Count = Arguments.FirstToken().AsInteger(1);
            Sides = Arguments.TokenAt(2).AsInteger(-1);

            if(Arguments.Contains("\""))
            {
                Before = Arguments.TokenAt(2, "\"");
                After = Arguments.TokenAt(4, "\"");
            }

            return true;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            string textToSend = null;
            if (ExtractedVerbs.Contains(Letters))
            {
                textToSend = SuperRandom.NextString(Count, true, false, false, false);
            }
            else if (ExtractedVerbs.Contains(Digits))
            {
                textToSend = SuperRandom.NextString(Count, false, true, false, false);
            }
            else if (ExtractedVerbs.Contains(Dice))
            {
                textToSend = SuperRandom.NextRoll(Count, Sides).ToString();
            }
            else if (ExtractedVerbs.Contains(Number))
            {
                textToSend = SuperRandom.NextLong(Count, Sides).ToString();
            }
            else
            {
                return true;
            }

            if(textToSend.IsNotEmpty())
            {
                if (Before.IsNotEmpty())
                    textToSend = Before + textToSend;
                if (After.IsNotEmpty())
                    textToSend += After;

                CustomPhraseManager.NormalSendKeysAndWait(textToSend);
            }
            return true;
        }
    }
}