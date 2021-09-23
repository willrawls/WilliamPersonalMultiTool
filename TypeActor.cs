using System;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;

namespace WilliamPersonalMultiTool
{
    public class TypeActor : BaseActor
    {
        public TypeActor(string item) : base(ActionType.Type, item)
        {
            if (item.IsEmpty()) return;
            
            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count > 1)
            {
                Verb = tokens[0];
            }
            else 
            {
                Verb = "expand";
            }

            if (Verb != "expand" && Verb != "percent")
                throw new Exception($"MoveActor: Invalid verb: {Arguments}");
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
}