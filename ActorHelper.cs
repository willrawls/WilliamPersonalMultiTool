﻿using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{
    public static class ActorHelper
    {
        public static List<BaseActor> ToActors(this ActionType actionType, List<string> relatedItems, List<PKey> keysToPrepend)
        {
            var actors = new List<BaseActor>();

            foreach(var item in relatedItems)
            {
                var actor = ToActor(actionType, item, keysToPrepend);

                if (actor == null)
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
                actors.Add(actor);
            }
            return actors;
        }

        public static BaseActor ToActor(this ActionType actionType, string item, List<PKey> keysToPrepend)
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
            
            if(actor != null && keysToPrepend?.Count > 0)
                actor.KeySequence.Sequence.InsertRange(0, keysToPrepend);

            return actor;
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

        public static KeyValuePair<string, ActionType> GetActionType(string line)
        {
            var lower = line.Replace("\r", "\n").ToLower();
            if (lower.StartsWith("when ")) lower = lower.TokensAfterFirst("when ");
            if (lower.StartsWith("or ")) lower = lower.TokensAfterFirst("or ");
            <<< Start here:
            //      Restructure dictionary into a ActionableList : List<Actionable>
            //      When not detected, assume it is a continuation (new parameter continuedFrom ?)
            foreach (var separator in Actionables.Where(separator => lower.Contains($" {separator.Key}")))
                return separator;
            return new KeyValuePair<string, ActionType>("unknown", ActionType.Unknown);
        }


    }
}