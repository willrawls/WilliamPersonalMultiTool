using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Phrase;
using Win32Interop.Structs;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class MoveActor : BaseActor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Verb Relative { get; set; }
        public Verb Percent { get; set; }   
        public Verb To { get; set; }        

        public MoveActor()
        {
            ActionableType = ActionableType.Move;

            // Move window to a specific location
            To = AddLegalVerb("to");                         
            
            // Move window by a certain amount
            Percent = AddLegalVerb("percent", To);            

            // With relative coordinates
            Relative = AddLegalVerb("relative");

            OnAct = Act;
        }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            var tokens = Arguments.AllTokens(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Count != 4)
            {
                Errors = "Move actor: wrong number of arguments";
                return false;
            }

            if(Relative.Mentioned)
            {
                Left = tokens[0].AsInteger();
                Top = tokens[1].AsInteger();
                Width = tokens[2].AsInteger();
                Height = tokens[3].AsInteger();

                Right = Left + Width;
                Bottom = Top + Height;
            }
            else // Absolute coordinates
            {
                Left = tokens[0].AsInteger();
                Top = tokens[1].AsInteger();
                Right = tokens[2].AsInteger();
                Bottom = tokens[3].AsInteger();

                Width = Right - Left;
                Height = Bottom - Top;
            }

            TargetScreen = 0;
            if (Percent.Mentioned)
            {
                while (Left > 100 || Right > 100 || Width > 100)
                {
                    TargetScreen++;
                    if(Left - 100 > 0)
                        Left -= 100;
                    if(Right - 100 > 0)
                        Right -= 100;
                    if(Right - Left > 0)
                        Width = Right - Left;
                }
            }

            return true;
        }

        public int TargetScreen { get; set; }

        public int Bottom { get; set; }

        public int Right { get; set; }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            if (Errors.IsNotEmpty())
                return true;

            return true;
        }

        public RECT CalculateNewPosition(RECT originalPosition)
        {
            RECT newPosition;
            if(Has(To)) // To or Percent
            {
                if(Has(Relative))
                {
                    // Additive
                    newPosition = new RECT
                    {
                        left = originalPosition.left + Left,
                        top = originalPosition.top + Top,
                        right = originalPosition.right + Width,
                        bottom = originalPosition.bottom + Height,
                    };
                }
                else
                {
                    // Absolute
                    newPosition = new RECT
                    {
                        left = Left,
                        top = Top,
                        right = Right,
                        bottom = Bottom,
                    };
                }
            }
            else if(Has(Relative))
            {
                // Percent == true
                // Relative
                Screen screen = Screen.AllScreens[TargetScreen];
                var onePercentX = screen.WorkingArea.Width / 100;
                var onePercentY = screen.WorkingArea.Height / 100;

                newPosition = new RECT
                {
                    left = originalPosition.left + (Left * onePercentX),
                    top = originalPosition.top + (Top * onePercentY),
                    right = originalPosition.right + (Right * onePercentX),
                    bottom = originalPosition.bottom + (Bottom * onePercentY),
                };
            }
            else
            {
                // Percent == true
                // Absolute
                Screen screen = Screen.AllScreens[TargetScreen];
                var onePercentX = screen.WorkingArea.Width / 100;
                var onePercentY = screen.WorkingArea.Height / 100;

                newPosition = new RECT
                {
                    left =  (Left * onePercentX),
                    top = (Top * onePercentY),
                    right = (Right * onePercentX),
                    bottom = (Bottom * onePercentY),
                };
            }

            return newPosition;
        }
    }
}