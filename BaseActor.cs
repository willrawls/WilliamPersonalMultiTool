using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Library.Generics;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public class BaseActor
    {
        public Action<string> ContinueWith = item => { };
        public Func<PhraseEventArguments, bool> OnAct = _ => false;
        public Func<string, bool> Factory { get; set; } = _ => false;

        public string KeyText { get; set; }
        public string Arguments { get; set; }
        public string Separator { get; set; }
        public string Errors { get; set; } = "";
        public bool CanContinue { get; set; }
        public Verb DefaultVerb { get; set; }

        public ActionableType ActionableType { get; set; }
        public List<Verb> ExtractedVerbs { get; set; } = new List<Verb>();
        public KeySequence KeySequence { get; set; }
        public static AssocArray<Verb> LegalVerbs { get; set; } = new AssocArray<Verb>();


        public virtual bool Initialize(string item)
        {
            var cleanItem = item
                .Replace("\r", "")
                .FirstToken("\n")
                .Trim();
            Separator = $" {ActionableType}";

            Arguments = cleanItem.TokensAfterFirst(Separator);

            if (Arguments.StartsWith(" "))
                Arguments = Arguments.Substring(1);

            KeyText = cleanItem.FirstToken(Separator);
            if (KeyText.ToLower().StartsWith("when ")) KeyText = KeyText.TokensAfterFirst("when ");
            if (KeyText.ToLower().StartsWith("or "))
                if (CanContinue)
                    KeyText = KeyText.TokensAfterFirst("or ");

            if (!ExtractVerbs(item))
                return false;

            KeySequence = new CustomKeySequence(KeyText, Arguments, this);

            return true;
        }

        public bool ExtractVerbs(string item)
        {
            var tokens = Arguments.AllTokens();
            var firstToken = tokens[0].Trim();

            var tokensToRemove = 0;
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                var verb = LegalVerbs[token].Item;
                if (LegalVerbs.ContainsKey(token))
                {
                    ExtractedVerbs.Add(verb);
                    tokens[i] = "";
                    tokensToRemove++;
                }
                else
                {
                    break; // First non verb stops the extraction
                }
            }

            if (tokensToRemove > 0)
                Arguments = Arguments.TokensAfter(tokensToRemove);

            // Check bound conditions here (too many, exclusivity)
            var exclusivityBreached = ExtractedVerbs
                .Any(v1 => v1
                               .Excludes != null
                           && ExtractedVerbs.Any(v2 => v2.Excludes != null && v2.Excludes.Name == v1.Excludes.Name));
            if (exclusivityBreached) Errors += "ExtractVerbs: Exclusivity breached";

            return true;
        }

        public static Verb AddLegalVerb(string name, Verb excludesVerb = null)
        {
            return LegalVerbs[name].Item = Verb.Factory(name, excludesVerb);
        }
    }
}