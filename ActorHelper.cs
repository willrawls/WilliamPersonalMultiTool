using System;
using System.Collections.Generic;

namespace WilliamPersonalMultiTool
{
    public static class ActorHelper
    {
        public static List<BaseActor> Factory(ActionType actionType, List<string> relatedItems)
        {
            var actors = new List<BaseActor>();

            foreach(var item in relatedItems)
            {
                BaseActor actor = null;
                switch (actionType)
                {
                    case ActionType.Type:
                        actor = new TypeActor(item);
                        break;

                    case ActionType.Size:
                        actor = new SizeActor(item);
                        break;

                    case ActionType.Move:
                        actor = new MoveActor(item);
                        break;

                    case ActionType.Repeat:
                        actor = new RepeatActor(item);
                        break;

                    case ActionType.Adjust:
                        actor = new AdjustActor(item);
                        break;

                    case ActionType.Run:
                        actor = new RunActor(item);
                        break;

                    case ActionType.Choose:
                        actor = new TypeActor(item);
                        break;
                }

                if (actor == null)
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
                actors.Add(actor);
            }
            return actors;
        }

        public static Dictionary<string, ActionType> Actionables = new Dictionary<string, ActionType>
        {
            {"type", ActionType.Type},
            {"run", ActionType.Run},
            {"choose", ActionType.Choose},
            {"move", ActionType.Move},
            {"size", ActionType.Size},
            {"adjust", ActionType.Adjust},

            {"unknown", ActionType.Unknown}
        };

        public static KeyValuePair<string, ActionType> GetActionType(string or)
        {
            var lower = or.Replace("\r", "\n").ToLower();
            foreach (KeyValuePair<string, ActionType> separator in Actionables)
                if (lower.Contains($" {separator.Key} ") || lower.Contains($" {separator.Key}\n"))
                    return separator;
            return new KeyValuePair<string, ActionType>("unknown", ActionType.Unknown);
        }


    }
}