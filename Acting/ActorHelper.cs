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
            Continuation = ActionableItem.WithActorFactory(() => new ContinuationActor());
            Unknown = ActionableItem.WithActorFactory(() => new UnknownActor());
        }

        public static ActionableItem Unknown { get; set; }

        public static ActionableItem Continuation { get; set; }

        public static ActionableItem GetActionType(string line)
        {
            var lower = line.Replace("\r", "\n").ToLower();
            if (lower.StartsWith("when ")) lower = lower.TokensAfterFirst("when ");
            if (lower.StartsWith("or ")) lower = lower.TokensAfterFirst("or ");

            return Actionables.ContainsKey(lower) 
                ? Actionables[lower].Item 
                : Continuation;
        }

        public static BaseActor Factory(string item, BaseActor previousActor = null)
        {
            var actionableItem = GetActionType(item);
            if (actionableItem == null) return null;

            var actor = actionableItem.Factory(item, previousActor);
            return actor is {ActionableType: ActionableType.Unknown} 
                ? null 
                : actor;
        }
    }
}