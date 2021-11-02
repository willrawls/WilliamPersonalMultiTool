using System;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public interface IAct
    {
        Func<bool> OnAct { get; set; }
        ActionType Action { get; }
        string Verb { get; set; }
        bool Act(PhraseEventArguments phraseEventArguments);
        KeySequence KeySequence { get; set; }

    }
}