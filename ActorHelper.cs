using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Library.Generics;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public static class ActorHelper
    {
        /*
        public static BaseActor ToActor(this ActionableType actionableType, string item, List<PKey> keysToPrepend)
        {
            BaseActor actor = null;
            switch (actionableType)
            {
                case ActionableType.Type:
                    actor = new TypeActor(item);
                    break;

                case ActionableType.Size:
                    actor = new SizeActor(item);
                    break;

                case ActionableType.Move:
                    actor = new MoveActor(item);
                    break;

                case ActionableType.Repeat:
                    actor = new RepeatActor(item);
                    break;

                case ActionableType.Adjust:
                    actor = new AdjustActor(item);
                    break;

                case ActionableType.Run:
                    actor = new RunActor(item);
                    break;

                case ActionableType.Choose:
                    actor = new TypeActor(item);
                    break;
            }
            
            if(actor != null && keysToPrepend?.Count > 0)
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);

            return actor;
        }*/

        /*public static Dictionary<string, ActionableType> oldActionables = new Dictionary<string, ActionableType>
        {
            {"type", ActionableType.Type},
            {"run", ActionableType.Run},
            {"choose", ActionableType.Choose},
            {"move", ActionableType.Move},
            {"size", ActionableType.Size},
            {"adjust", ActionableType.Adjust},
            
            {"unknown", ActionableType.Unknown}
        };*/

        public static Actionable Actionables;

        static ActorHelper()
        {
            Actionables.AddRange(new []
            {
                new ActionableItem().WithActorFactory(() => new TypeActor()),
                new ActionableItem().WithActorFactory(() => new ChooseActor()),
                new ActionableItem().WithActorFactory(() => new MoveActor()),
                new ActionableItem().WithActorFactory(() => new RunActor()),
                new ActionableItem().WithActorFactory(() => new SizeActor()),
                new ActionableItem().WithActorFactory(() => new AdjustActor()),
                new ActionableItem().WithActorFactory(() => new ContinuationActor()),
                new ActionableItem().WithActorFactory(() => new UnknownActor()),
            });

     
            
            
            Actionables[item.ActionableType.ToString().ToLower()].Item = item;

            Actionables["type"].Item.WithActorFactory(false, () => { return new ActionableItem(); });

            Actionables["unknown"].Item.ActionableType = ActionableType.Type;
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

        public static BaseActor Factory(string item, List<PKey> keysToPrepend = null)
        {
            ActionableItem actionableItem = GetActionType(item);
            if (actionableItem == null) return null;

            BaseActor actor = actionableItem.Factory(item, keysToPrepend);
            if (actor.ActionableType == ActionableType.Unknown) return null;

            return actor;

        }
    }
}