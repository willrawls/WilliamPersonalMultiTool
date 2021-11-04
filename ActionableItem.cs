using System;
using System.Collections.Generic;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public class ActionableItem 
    {
        public ActionableType ActionableType { get; set; }
        public Type ActorType { get; set; }

        public ActionableItem()
        {
        }

        public ActionableItem(ActionableType actionableType, Type actorType)
        {
            ActionableType = actionableType;
            ActorType = actorType;
        }

        public BaseActor ToActor(string item, List<PKey> keysToPrepend)
        {
            var actor = Activator.CreateInstance(ActorType) as BaseActor;
            actor?.Initialize(ActionableType, item, keysToPrepend);

            return actor;
        }

    }
}