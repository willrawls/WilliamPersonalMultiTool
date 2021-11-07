using System;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class ChooseActor : BaseActor
    {
        public ChooseActor()
        {
            base.CanContinue = true;
        }

        public override bool Act(PhraseEventArguments phraseEventArguments)
        {
            return true;
        }

        public void ContinueWith(string item)
        {
            if (item.IsEmpty()) return;

            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            Verb = tokens.Count > 0
                ? tokens[0]
                : "default";

            if (item.Trim().StartsWith("Or"))
            {
                var actionableItem = ActorHelper.GetActionType(item);
                var keysToPrepend = KeySequence.Sequence.Take(KeySequence.Sequence.Count - 1).ToList();
                return actionableItem.ToActor(item.TokensAfterFirst("Or"), keysToPrepend);
            }

            return actor;
        }

    }
}