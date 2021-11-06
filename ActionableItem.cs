﻿using System;
using System.Collections.Generic;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public class ActionableItem
    {
        public ActionableType ActionableType { get; set; }
        public bool CanContinue {get; set; }

        public BaseActor Factory(string item, List<PKey> keysToPrepend = null)
        {
            if (InternalFactory == null)
                return null;

            BaseActor actor = InternalFactory(item, keysToPrepend);
            actor.Initialize(item, keysToPrepend);
            return actor;
        }
        public Func<string, List<PKey>, BaseActor> InternalFactory;

        public ActionableItem Initialize(bool canContinue, Func<string, List<PKey>, BaseActor> factory)
        {
            CanContinue = canContinue;
            InternalFactory = factory;
            return this;
        }

        public List<BaseActor> ToActors(List<string> relatedItems, List<PKey> keysToPrepend)
        {
            var actors = new List<BaseActor>();

            foreach(var item in relatedItems)
            {
                var actor = Factory(item, keysToPrepend);
                if (actor == null)
                    throw new ArgumentOutOfRangeException(nameof(ActionableType), ActionableType, null);
                actors.Add(actor);
            }
            return actors;
        }
    }

    /*public class ActionableItem<T> : ActionableItem where T: BaseActor, new()
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
            actor.Initialize(item);

            if (keysToPrepend.IsNotEmpty())
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);

            return actor;
        }

    }*/
}