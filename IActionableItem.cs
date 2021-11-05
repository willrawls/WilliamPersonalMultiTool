using System;
using System.Collections.Generic;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public interface IActionableItem
    {
        ActionableType ActionableType { get; set; }
        Type ActorType { get; set; }
        BaseActor ToActor(string item, List<PKey> keysToPrepend);
        ActionableItem InitializeActor(ActionableType actionableType, Type actorType);
        List<BaseActor> ToActors(List<string> relatedItems, List<PKey> keysToPrepend);
    }
}