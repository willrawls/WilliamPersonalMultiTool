using System;
using System.Collections.Generic;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public class ActionableItem
    {
        public string Fred = "fred";
    }

    public class ActionableItem<T> : ActionableItem where T: BaseActor, new()
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
            T actor = Activator.CreateInstance<T>();
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