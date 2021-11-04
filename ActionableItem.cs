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
            object actorObject = Activator.CreateInstance(ActorType);
            BaseActor actor = actorObject as BaseActor;
            actor?.Initialize(ActionableType, item, keysToPrepend);
            return actor;
        }

        public List<BaseActor> ToActors(List<string> relatedItems, List<PKey> keysToPrepend)
        {
            var actors = new List<BaseActor>();

            foreach(var item in relatedItems)
            {
                var actor = ToActor(item, keysToPrepend);
                if (actor == null)
                    throw new ArgumentOutOfRangeException(nameof(ActionableType), ActionableType, null);
                actors.Add(actor);
            }
            return actors;
        }


    }
}