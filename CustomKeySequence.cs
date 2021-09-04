using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetX.Standard.Library;
using Microsoft.Win32;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using Win32Interop.Enums;

namespace WilliamPersonalMultiTool
{
    public class CustomKeySequence : KeySequence
    {
        public int BackspaceCount { get; set; }
        public string Arguments { get; set; }
        public string ExecutablePath { get; set; }
        public Color BackColor { get; set; } = Color.White;
        public List<CustomKeySequenceChoice> Choices { get; set; }
        public IAct Actor { get; set; }

        public CustomKeySequence(string name, List<PKey> keys, EventHandler<PhraseEventArguments> hotPhraseEventArgs, int backspaceCount = 0, Color? backColor = null) 
            : base(name, keys, hotPhraseEventArgs)
        {
            if (backspaceCount < 0)
                backspaceCount = 0;
            BackspaceCount = backspaceCount;
            BackColor = backColor ?? Color.White;
        }
    }

    public class CustomKeySequenceChoice
    {
        public string Text { get; set; }
    }

    public class SizeActor : BaseActor
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public SizeActor(KeySequence keySequence, string arguments) : base(ActionType.Size, keySequence)
        {
            KeySequence = keySequence;
            if (arguments.IsEmpty()) return;
            
            var tokens = arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count == 3)
            {
                Verb = tokens[0].Replace("%", "percent");
                Width = tokens[1].AsInteger();
                Height = tokens[2].AsInteger();
            }
            else if (tokens.Count > 0) // Default is "to"
            {
                Verb = "by";
                if(tokens.Count > 2) Width = tokens[0].AsInteger();
                if(tokens.Count > 3) Height = tokens[1].AsInteger();
            }

            if (Verb != "by" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {arguments}");
        }

        public override bool Act()
        {
            return true;
        }
    }

    public class MoveActor : BaseActor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public MoveActor(KeySequence keySequence, string arguments) : base(ActionType.Move, keySequence)
        {
            if (arguments.IsEmpty()) return;
            
            var tokens = arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count == 5)
            {
                Verb = tokens[0].Replace("%", "percent");
                Left = tokens[1].AsInteger();
                Top = tokens[2].AsInteger();
                Width = tokens[3].AsInteger();
                Height = tokens[4].AsInteger();
            }
            else if (tokens.Count > 0) // Default is "to"
            {
                Verb = "to";
                if(tokens.Count > 0) Left = tokens[0].AsInteger();
                if(tokens.Count > 1) Top = tokens[1].AsInteger();
                if(tokens.Count > 2) Width = tokens[2].AsInteger();
                if(tokens.Count > 3) Height = tokens[3].AsInteger();
            }

            if (Verb != "to" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {arguments}");
        }

        public override bool Act()
        {
            if (Verb.IsEmpty())
                return false;

            switch (Verb.ToLower())
            {
                case "to":
                    break;

                case "%":
                case "percent":
                    break;

                default:
                    break;
            }
            return true;
        }
    }

    public class RepeatActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public RepeatActor(KeySequence keySequence, string arguments) : base(ActionType.Repeat, keySequence)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }

    public static class ActorHelper
    {
        public static BaseActor Factory(ActionType actionType, string arguments)
        {
            var keySequence = new KeySequence();
            switch(actionType)
            {
                case ActionType.Type:
                    return new TypeActor(keySequence, arguments);
                case ActionType.Size:
                    return new SizeActor(keySequence, arguments);
                case ActionType.Move:
                    return new MoveActor(keySequence, arguments);
                case ActionType.Repeat:
                    return new RepeatActor(keySequence, arguments);
                case ActionType.Adjust:
                    return new AdjustActor(keySequence, arguments);
                case ActionType.Run:
                    return new RunActor(keySequence, arguments);
                case ActionType.Choose:
                    return new TypeActor(keySequence, arguments);
            }
            throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
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
                if (lower.Contains($" {separator} ") || lower.Contains($" {separator}\n"))
                    return separator;
            return new KeyValuePair<string, ActionType>("unknown", ActionType.Unknown);
        }


    }

    public class RunActor : BaseActor
    {
        public string TargetExecutable { get; set; }
        public string Arguments { get; set; }

        public RunActor(KeySequence keySequence, string arguments) : base(ActionType.Run, keySequence)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }

    public class ChooseActor : BaseActor
    {
        public ChooseActor(KeySequence keySequence) : base(ActionType.Choose, keySequence)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }

    public class AdjustActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public AdjustActor(KeySequence keySequence, string arguments) : base(ActionType.Adjust, keySequence)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }

    public class TypeActor : BaseActor
    {
        public TypeActor(KeySequence keySequence, string arguments) : base(ActionType.Type, keySequence)
        {
            if (arguments.IsEmpty()) return;
            
            var tokens = arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count == 1)
            {
                Verb = tokens[0];
            }
            else 
            {
                Verb = "expand";
            }

            if (Verb != "expand" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {arguments}");
        }

        public string TextToPaste()
        {
            var text = Clipboard.GetText();
            return text;
        }

        public override bool Act()
        {
            return true;
        }
    }

    public abstract class BaseActor : IAct
    {
        protected BaseActor(ActionType actionType, KeySequence keySequence)
        {
            Action = actionType;
            KeySequence = keySequence;
        }

        public Func<bool> OnAct { get; set; }
        public ActionType Action { get; set; }
        public string Verb { get; set; }
        public KeySequence KeySequence { get; set; }

        public virtual bool Act()
        {
            return OnAct == null || OnAct();
        }
    }

    public interface IAct
    {
        Func<bool> OnAct { get; set; }
        ActionType Action { get; }
        string Verb { get; set; }
        bool Act();
        KeySequence KeySequence { get; set; }

    }

    public enum ActionType
    {
        Unknown,
        Type,
        Size,
        Move,
        Repeat,
        Adjust,
        Run,
        Choose
    }

}