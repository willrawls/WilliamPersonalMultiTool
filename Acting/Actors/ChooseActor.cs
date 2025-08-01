﻿using MetX.Standard.Strings;
using MetX.Standard.Strings.Tokens;
using NHotPhrase.Phrase;
using System.Collections.Generic;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class ChooseActor : BaseActor
    {
        public List<string> Choices { get; set; } = new List<string>();
        public ChooseActor()
        {
            ActionableType = ActionableType.Choose;
            CanContinue = true;
            OnAct = Act;
            OnContinue = Continue;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            return false;
        }

        // Return true to continue to the next line
        public bool Continue(string item)
        {
            if (item.Trim().StartsWith("Or "))
                item = item.TokensAfterFirst("Or ");

            if (item.IsEmpty()) return true;

            Choices.Add(item.TrimStart());

            return true;
        }

    }
}