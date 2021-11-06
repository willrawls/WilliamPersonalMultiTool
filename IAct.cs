using System;
using System.Collections.Generic;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public interface IAct
    {
        Func<bool> OnAct { get; set; }
        Func<string, bool> Factory { get; set; }
        ActionableType ActionableType { get; }
        List<string> Verbs { get; set; }
        bool Act(PhraseEventArguments phraseEventArguments);
        KeySequence KeySequence { get; set; }

    }
}