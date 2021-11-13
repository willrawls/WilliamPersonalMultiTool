using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library.Extensions;
using MetX.Standard.Library.Generics;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool.Acting
{
    public class ActionableItem
    {
        public Func<BaseActor> InternalFactory;

        public BaseActor Factory(string item, BaseActor previousActor = null)
        {
            if (InternalFactory == null)
                return null;

            var actor = InternalFactory();
            actor.Initialize(item);

            if (previousActor == null) return actor;

            var keysToPrepend = previousActor.KeySequence
                .Sequence
                .Take(previousActor.KeySequence.Sequence.Count - 1)
                .ToList();

            if (keysToPrepend.IsNotEmpty())
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);

            return actor;
        }

        public static ActionableItem WithActorFactory(Func<BaseActor> factory)
        {
            var sampleActor = factory();

            var assocItem = ActorHelper.Actionables[sampleActor.ActionableType.ToString()];
            assocItem.Item.InternalFactory = factory;
            assocItem.Name = assocItem.Key;
            return assocItem.Item;
        }

    }
}