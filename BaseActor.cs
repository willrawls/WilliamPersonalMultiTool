using System;
using MetX.Standard.Library;
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
                .FirstToken("\n");
            Separator = $" {Action}";
            Arguments = cleanItem.TokensAfterFirst(Separator);
            KeyText = cleanItem.FirstToken(Separator);
            KeySequence = new KeySequence(KeyText);
        }

        public string KeyText { get; set; }

        public string Arguments { get; set; }
        public string Separator { get; set; }

        public Func<bool> OnAct { get; set; }
        public ActionType Action { get; set; }
        public string Verb { get; set; }
        public KeySequence KeySequence { get; set; }

        public virtual bool Act()
        {
            return OnAct == null || OnAct();
        }
    }
}