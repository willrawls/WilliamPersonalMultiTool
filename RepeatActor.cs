﻿using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
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