using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class ChooseActor : BaseActor
    {
        private List<string> Choices { get; set; } = new List<string>();
        public ChooseActor()
        {
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