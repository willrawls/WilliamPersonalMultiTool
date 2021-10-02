using System.Collections.Generic;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class BaseActorList : List<BaseActor>
    {
        public KeySequenceList KeySequences { get; set; }
    }
}