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
            Actionables = new Actionable
            {
                ["type"] = {Item = new ActionableItem(ActionableType.Type, typeof(TypeActor))},
                ["run"] = {Item = new ActionableItem(ActionableType.Run, typeof(RunActor))},
                ["choose"] = {Item = new ActionableItem(ActionableType.Choose, typeof(ChooseActor))},
                ["move"] = {Item = new ActionableItem(ActionableType.Move, typeof(MoveActor))},
                ["Size"] = {Item = new ActionableItem(ActionableType.Size, typeof(SizeActor))},
                ["adjust"] = {Item = new ActionableItem(ActionableType.Adjust, typeof(AdjustActor))},

                ["continuation"] = {Item = new ActionableItem(ActionableType.Adjust, typeof(ContinuationActor))},
            };

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

    }
}