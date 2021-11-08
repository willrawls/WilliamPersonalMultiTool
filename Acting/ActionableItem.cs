using System;
using System.Collections.Generic;
using MetX.Standard.Library.Extensions;
using MetX.Standard.Library.Generics;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Acting
{
    public class ActionableItem
    {
        public Func<BaseActor> InternalFactory;

        public BaseActor Factory(string item, List<PKey> keysToPrepend = null)
        {
            if (InternalFactory == null)
                return null;

            var actor = InternalFactory();
            actor.Initialize(item);

            if (keysToPrepend.IsNotEmpty())
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);
            
            return actor;
        }

        public static ActionableItem WithActorFactory(Func<BaseActor> factory)
        {
            var assocItem = new AssocItem<ActionableItem>();
            assocItem.Item.InternalFactory = factory;
            ActorHelper.Actionables[assocItem.Key] = assocItem;
            return assocItem.Item;
        }

        public List<BaseActor> ToActors(List<string> relatedItems, List<PKey> keysToPrepend)
        {
            var actors = new List<BaseActor>();

            foreach (var item in relatedItems)
            {
                var actor = Factory(item, keysToPrepend);
                if (actor == null)
                    throw new Exception($"Actor factory failed: {item}");
                actors.Add(actor);
            }

            return actors;
        }
    }
}