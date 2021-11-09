using System.Collections.Generic;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting.Actors;

namespace WilliamPersonalMultiTool.Acting
{
    public static class ActorHelper
    {
        public static Actionables Actionables;

        static ActorHelper()
        {
            Actionables = new Actionables();

            ActionableItem.WithActorFactory(() => new TypeActor());
            ActionableItem.WithActorFactory(() => new ChooseActor());
            ActionableItem.WithActorFactory(() => new MoveActor());
            ActionableItem.WithActorFactory(() => new RunActor());
            ActionableItem.WithActorFactory(() => new SizeActor());
            ActionableItem.WithActorFactory(() => new RandomActor());
            ActionableItem.WithActorFactory(() => new ContinuationActor());
            ActionableItem.WithActorFactory(() => new UnknownActor());
        }

        public static ActionableItem GetActionType(string line)
        {
            var lower = line.Replace("\r", "\n").ToLower();
            if (lower.StartsWith("when ")) lower = lower.TokensAfterFirst("when ");
            if (lower.StartsWith("or ")) lower = lower.TokensAfterFirst("or ");

            if (Actionables.ContainsKey(lower))
                return Actionables[lower].Item;
            return Actionables["continuation"].Item;
        }

        public static BaseActor Factory(string item, BaseActor previousActor = null)
        {
            ActionableItem actionableItem = GetActionType(item);
            if (actionableItem == null) return null;

            BaseActor actor = actionableItem.Factory(item, previousActor);
            if (actor.ActionableType == ActionableType.Unknown) return null;

            return actor;

        }
    }
}