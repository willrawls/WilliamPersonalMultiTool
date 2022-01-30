using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Custom;
using System.Collections.Generic;

namespace WilliamPersonalMultiTool
{
    public class InlinePieceList : System.Collections.Generic.List<InlinePiece> 
    {
        public CustomPhraseManager Manager { get; }

        public InlinePieceList(CustomPhraseManager manager, string target)
        {
            Manager = manager;
            var pieces = target.Splice("{", "}");
            var isOdd = true;
            foreach (var piece in pieces)
            {
                if (isOdd)
                {
                    isOdd = false;
                    this.Add(new InlinePiece(piece, false));
                }
                else
                {
                    var isCommand = piece.Contains(" ") || piece.ToLower().Contains("clipboard");
                    var inlinePiece = new InlinePiece(piece, isCommand);
                    this.Add(inlinePiece);
                    if(!isCommand)
                    {
                        var entry = SendKeyHelper.Entries.FirstOrDefault(x => string.Equals(x.Name, piece, StringComparison.InvariantCultureIgnoreCase));
                        if (entry != null)
                        {
                            inlinePiece.Command = "pkey";
                        }
                    }
                    isOdd = true;
                }
            }
        }

        public void Play()
        {
            foreach (InlinePiece piece in this)
            {
                if (piece.Command.IsEmpty())
                {
                    Manager.NormalSendKeysAndWait(piece.Contents);
                }
                else
                {
                    switch (piece.Command)
                    {
                        case "pause":
                            var milliseconds = int.Parse(piece.Arguments);
                            if(milliseconds > 0)
                                Thread.Sleep(milliseconds);
                            break;

                        case "clipboard":
                            var textToType = Clipboard.GetText();
                            if (textToType.IsEmpty())
                                break;

                            Manager.SendString(textToType, 5, true);
                            break;

                        case "guid":
                            var format = piece.Arguments.IsEmpty() ? "N" : piece.Arguments;
                            var guid = Guid.NewGuid().ToString(format);
                            Manager.SendString(guid, 5, true);
                            break;

                        case "roll":
                            var count = int.Parse(piece.Arguments.FirstToken());
                            var sides = int.Parse(piece.Arguments.TokenAt(2));
                            var textToSend = SuperRandom.NextRoll(count, sides).ToString();
                            Manager.SendString(textToSend, 5, true);
                            break;

                        case "pkey":
                            Manager.SendString($"{{{piece.Contents}}}", 5, false);
                            break;
                    }
                }
            }
        }
    }
}