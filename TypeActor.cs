using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class TypeActor : BaseActor
    {
        static TypeActor()
        {
            // Type everything else, allowing send key translation (The default)
            Expand = AddLegalVerb("expand");

            // Types any text already on the clipboard
            TypeOutTheClipboard = AddLegalVerb("clipboard");

            // Types the contents of a file
            TypeContentsOfFile = AddLegalVerb("file", TypeOutTheClipboard);

            // Causes longer and more frequent delays while typing
            TypeSlow = AddLegalVerb("slow");

            // Causes longer and more frequent delays while typing
            TypeSlower = AddLegalVerb("slower", TypeSlow);

            // Causes longer and more frequent delays while typing
            TypeSlowest = AddLegalVerb("slowest", TypeSlower);

            // Causes quick typing (Fast is the default)
            TypeFast = AddLegalVerb("fast", TypeSlowest);
        }

        public static Verb Expand { get; set; }
        public static Verb TypeOutTheClipboard { get; set; }
        public static Verb TypeContentsOfFile { get; set; }
        public static Verb TypeFast { get; set; }
        public static Verb TypeSlow { get; set; }
        public static Verb TypeSlowest { get; set; }
        public static Verb TypeSlower { get; set; }

        public string Filename { get; set; }
        public string TextToType { get; set; }
        public bool FromClipboard { get; set; }
        public int DelayInMilliseconds { get; set; } = 3;

        public TypeActor()
        {
            OnAct = Act;
            DefaultVerb = Expand;
            CanContinue = true;
        }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            if (ExtractedVerbs.Contains(TypeContentsOfFile))
                Filename = Arguments;
            else if (ExtractedVerbs.Contains(TypeOutTheClipboard))
                FromClipboard = true;
            else
            {
                // Expand
                TextToType = Arguments;
            }

            DelayInMilliseconds =
                ExtractedVerbs.WhenContains(TypeSlowest, 50) +
                ExtractedVerbs.WhenContains(TypeSlower, 10) +
                ExtractedVerbs.WhenContains(TypeSlow, 3) +
                ExtractedVerbs.WhenContains(TypeFast, 1);

            KeySequence.Name = Arguments.Trim();
            return true;
        }

        public string TextToPaste()
        {
            var text = Clipboard.GetText();
            return text;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            
        }
    }
}