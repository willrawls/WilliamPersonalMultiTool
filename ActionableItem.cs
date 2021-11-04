using System;
using System.Collections.Generic;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public abstract class ActionableItem
    {
        public ActionableType ActionableType { get; set; }
        public Type ActorType { get; set;  }

        public ActionableItem()
        {
        }

        /*
        public ActionableItem(ActionableType actionableType, Type actorType)
        {
            ActionableType = actionableType;
            ActorType = actorType;
        }
        */

        public abstract BaseActor ToActor(string item, List<PKey> keysToPrepend);

        public ActionableItem InitializeActor(ActionableType actionableType, Type actorType)
        {
            ActionableType = actionableType;
            ActorType = actorType;
            InitializeBase();
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

    public class ActionableItem<T> : ActionableItem where T: BaseActor, new()
    {
        public ActionableItem()
        {
        }

        public ActionableItem(ActionableType actionableType)
        {
            ActionableType = actionableType;
            ActorType = typeof(T);
        }

        public override BaseActor ToActor(string item, List<PKey> keysToPrepend)
        {
            var actor = new T();
            actor.InitializeActor(item);

            if (keysToPrepend.IsNotEmpty())
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);

            return actor;
        }

    }
}