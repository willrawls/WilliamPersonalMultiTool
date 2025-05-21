using MetX.Standard.Strings;
using MetX.Standard.Strings.Tokens;
using NHotPhrase.Phrase;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class MoveActor : BaseActor
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Verb Add { get; set; }
        public Verb To { get; set; }
        public Verb Relative { get; set; }
        public Verb Resize { get; set; }
        public Verb Percent { get; set; }

        public MoveActor()
        {
            ActionableType = ActionableType.Move;

            Add = AddLegalVerb("add");
            To = AddLegalVerb("to");
            Percent = AddLegalVerb("percent", To);
            Relative = AddLegalVerb("relative");
            Resize = AddLegalVerb("size");

            OnAct = Act;
        }

        //
        //  Add      = Add/subtract to/from current position and optionally resize on current or target screen
        //  To       = Move to position and optionally resize on current or target screen
        //  Relative = Move to relative position and optionally resize on current or target screen
        //  Resize     = Resize on current or target screen
        //             2 Coordinates= width, height 
        //
        //Modifiers
        // +Percent  = Coordinates are percentages of the target/current screen
        //
        // 2 coordinates= left, top
        // 3 coordinates= left, top, screen
        // 4 coordinates= left, top, width, height   (primary screen)
        // 5 coordinates= left, top, width, height, screen
        //

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

            if (Resize.Mentioned)
            {
                if (tokens.Count == 2)
                {
                    Width = tokens[0].AsInteger();
                    Height = tokens[1].AsInteger();
                }
                else if (tokens.Count == 3)
                {
                    Width = tokens[0].AsInteger();
                    Height = tokens[1].AsInteger();
                    TargetScreen = tokens[2].AsInteger() - 1;
                }
                else
                {
                    throw new Exception("Too many or too few parameters for the mentioned verbs.");
                }
            }
            else if (tokens.Count == 4)
            {
                Left = tokens[0].AsInteger();
                Top = tokens[1].AsInteger();
                Width = tokens[2].AsInteger();
                Height = tokens[3].AsInteger();
                Right = Left + Width;
                Bottom = Top + Height;
            }
            else if (tokens.Count == 5)
            {
                Left = tokens[0].AsInteger();
                Top = tokens[1].AsInteger();
                Width = tokens[2].AsInteger();
                Height = tokens[3].AsInteger();
                TargetScreen = tokens[4].AsInteger() - 1;

                Right = Left + Width;
                Bottom = Top + Height;
            }
            else
            {
                throw new Exception("Too many or too few parameters for the mentioned verbs.");
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

            var origin = WindowWorker.GetForegroundWindowPosition();
            if (!origin.HasValue) return false;

            var mousePosition = Cursor.Position;
            var currentScreen = Screen.FromPoint(mousePosition);
            var currentScreenIndex = currentScreen.Index();
            var newPosition = CalculateNewPosition(currentScreenIndex, origin.Value);
            WindowWorker.MoveForegroundWindowTo(newPosition);
            return true;
        }

        public Rectangle CalculateNewPosition(int currentScreen, Rectangle origin)
        {
            var targetScreen = TargetScreen < 1
                ? Screen.AllScreens[currentScreen]
                : Screen.AllScreens[TargetScreen];

            if (Add.Mentioned)
            {
                return CalculateNewAddPosition(origin, targetScreen);
            }

            if (To.Mentioned)
            {
                return CalculateNewToPosition(targetScreen);
            }

            if (Relative.Mentioned)
            {
                return CalculateNewRelativePosition(origin, targetScreen);
            }

            if (Resize.Mentioned)
            {
                return CalculateNewSize(origin, targetScreen);
            }

            Rectangle newPosition;
            if (To.Mentioned) // To means not Percent
            {
                if (Relative.Mentioned)
                {
                    // To + Relative (-Percent)
                    // Additive
                    newPosition = new Rectangle(
                        origin.Left + Left,
                        origin.Top + Top,
                        origin.Right + Width,
                        origin.Bottom + Height);
                }
                else
                {
                    // To alone (-Percent -Relative)
                    // Absolute
                    newPosition = new Rectangle(Left, Top, Width, Height);
                }
            }
            else if (Relative.Mentioned)
            {
                // Relative alone (-Top +Percent)
                // Percent == true
                // Relative
                var screen = Screen.AllScreens[TargetScreen];
                var onePercentX = screen.WorkingArea.Width / 100;
                var onePercentY = screen.WorkingArea.Height / 100;

                newPosition = new Rectangle(
                    origin.Left + (Left * onePercentX),
                    origin.Top + (Top * onePercentY),
                    origin.Right + (Width * onePercentX),
                    origin.Bottom + (Height * onePercentY));
            }
            else
            {
                // Percent == true (-Top -Relative)
                // Absolute percent
                var onePercentX = targetScreen.PercentX();
                var onePercentY = targetScreen.PercentY();

                newPosition = new Rectangle(
                    Left * onePercentX,
                    Top * onePercentY,
                    Width * onePercentX,
                    Height * onePercentY);
            }

            return newPosition;
        }

        private Rectangle CalculateNewSize(Rectangle origin, Screen targetScreen)
        {
            Rectangle newPosition;

            if (Percent.Mentioned)
            {
                var onePercentX = targetScreen.WorkingArea.Width / 100;
                var onePercentY = targetScreen.WorkingArea.Height / 100;

                newPosition = new Rectangle(
                    Left * onePercentX,
                    Top * onePercentY,
                    Width * onePercentX,
                    Height * onePercentY);
            }
            else
            {
                newPosition = new Rectangle(origin.Left + Left, origin.Top + Top,
                    origin.Right + Width,
                    origin.Bottom + Height);
            }

            return newPosition;
        }

        private Rectangle CalculateNewRelativePosition(Rectangle origin, Screen targetScreen)
        {
            if (!Percent.Mentioned)
                return new Rectangle(
                    origin.Left + Left,
                    origin.Top + Top,
                    origin.Left + Left + (origin.Right - Left) + Width,
                    origin.Top + Top + (origin.Bottom - origin.Top) + Height);

            var onePercentX = ((int)(targetScreen.Bounds.Width / 100f));
            var onePercentY = ((int)(targetScreen.Bounds.Height / 100f));

            var relativeLeft = onePercentX * Left;
            var relativeRight = onePercentX * Right;
            var relativeTop = onePercentY * Top;
            var relativeBottom = onePercentY * Bottom;
            var relativeWidth = relativeRight - relativeLeft;
            var relativeHeight = relativeBottom - relativeTop;

            return WindowWorker.NewRectangle(
                origin.Left + relativeLeft,
                origin.Top + relativeTop,
                origin.Left + relativeLeft + (origin.Right - origin.Left) + relativeWidth,
                origin.Top + relativeTop + (origin.Bottom - origin.Top) + relativeHeight
            );
        }

        private Rectangle CalculateNewToPosition(Screen targetScreen)
        {
            if (!Percent.Mentioned)
                return WindowWorker.NewRectangle(Left, Top, Right, Bottom);


            var onePercentX = ((int)(targetScreen.Bounds.Width / 100f));
            var onePercentY = ((int)(targetScreen.Bounds.Height / 100f));

            var relativeLeft = onePercentX * Left;
            var relativeRight = onePercentX * Right;
            var relativeTop = onePercentY * Top;
            var relativeBottom = onePercentY * Bottom;

            return WindowWorker.NewRectangle(relativeLeft, relativeTop, relativeRight, relativeBottom);
        }

        private Rectangle CalculateNewAddPosition(Rectangle origin, Screen targetScreen)
        {
            if (!Percent.Mentioned)
                return WindowWorker.NewRectangle(
                    origin.Left + Left,
                    origin.Top + Top,
                    origin.Left + Left + (origin.Right - origin.Left) + Width,
                    origin.Top + Top + (origin.Bottom - origin.Top) + Height);

            var onePercentX = ((int)(targetScreen.Bounds.Width / 100f));
            var onePercentY = ((int)(targetScreen.Bounds.Height / 100f));

            var relativeLeft = onePercentX * Left;
            var relativeRight = onePercentX * Right;
            var relativeTop = onePercentY * Top;
            var relativeBottom = onePercentY * Bottom;
            var relativeWidth = relativeRight - relativeLeft;
            var relativeHeight = relativeBottom - relativeTop;

            return WindowWorker.NewRectangle(
                origin.Left + relativeLeft,
                origin.Top + relativeTop,
                origin.Left + relativeLeft + (origin.Right - origin.Left) + relativeWidth,
                origin.Top + relativeTop + (origin.Bottom - origin.Top) + relativeHeight);
        }
    }
}