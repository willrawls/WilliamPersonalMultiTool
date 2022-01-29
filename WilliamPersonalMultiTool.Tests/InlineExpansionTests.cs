using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHotPhrase.Keyboard;
using WilliamPersonalMultiTool.Acting.Actors;
using WilliamPersonalMultiTool.Custom;

namespace WilliamPersonalMultiTool.Tests
{
    [TestClass]
    public class InlineExpansionTests
    {
        [TestMethod]
        public void Pause5Milliseconds()
        {
            var actual = new InlinePieceList("ab{pause 5}cd");

            Assert.AreEqual(3, actual.Count);
            
            Assert.AreEqual("ab", actual[0].Contents);
            Assert.IsNull(actual[0].Command);
            Assert.IsNull(actual[0].Arguments);

            Assert.AreEqual("pause 5", actual[1].Contents);
            Assert.AreEqual("pause", actual[1].Command);
            Assert.AreEqual("5", actual[1].Arguments);

            Assert.AreEqual("cd", actual[2].Contents);
            Assert.IsNull(actual[2].Command);
            Assert.IsNull(actual[2].Arguments);
        }
    }

    public class InlinePiece
    {
        public string Contents { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }

        public InlinePiece(string contents, bool isCommand)
        {
            Contents = contents;
            if (!isCommand) return;

            Command = contents.FirstToken().ToLower();
            Arguments = contents.TokensAfterFirst();
        }

        public override string ToString()
        {
            return Contents;
        }
    }

    public class InlinePieceList : List<InlinePiece> 
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
                    var isCommand = piece.Contains(" ");
                    InlinePiece p = this.Add(new InlinePiece(piece, isCommand));
                    if(!isCommand)
                    {
                        var entry = SendKeyHelper.Entries.FirstOrDefault(x => string.Equals(x.Name, piece, StringComparison.InvariantCultureIgnoreCase));
                        if (entry != null)
                        {
                            p.Command = "pkey";
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