using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Metadata;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public abstract class BaseActor : IAct
    {
        protected BaseActor(ActionType actionType, string item)
        {
            Action = actionType;
            var cleanItem = item
                .Replace("\r", "")
                .FirstToken("\n")
                .Trim();
            Separator = $" {Action}";
            
            Arguments = cleanItem.TokensAfterFirst(Separator);
            if (Arguments.StartsWith(" "))
                Arguments = Arguments.Substring(1);

            KeyText = cleanItem.FirstToken(Separator);
            if(KeyText.ToLower().StartsWith("when "))
                KeyText = KeyText.TokensAfterFirst("when ");
            if(KeyText.ToLower().StartsWith("or "))
                KeyText = KeyText.TokensAfterFirst("or ");
            
            KeySequence = new CustomKeySequence(KeyText, Arguments, this);
        }

        public string KeyText { get; set; }

        public string Arguments { get; set; }
        public string Separator { get; set; }

        public Func<bool> OnAct { get; set; }
        public ActionType Action { get; set; }
        public string Verb { get; set; }
        public KeySequence KeySequence { get; set; }

        public virtual bool Act(PhraseEventArguments phraseEventArguments)
        {
            return OnAct == null || OnAct();
        }

        public BaseActor ContinueWith(string line)
        {
            BaseActor actor = null;
            if (line.Trim().StartsWith("Or"))
            {
                var actionTypeEntry = ActorHelper.GetActionType(line);
                var keysToPrepend = KeySequence.Sequence.Take(KeySequence.Sequence.Count - 1).ToList();
                return actionTypeEntry.Value.ToActor(line.TokensAfterFirst("Or"), keysToPrepend);
            }

            return actor;
        }
    }
}