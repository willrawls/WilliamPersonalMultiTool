using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using NHotPhrase.WindowsForms;

namespace WilliamPersonalMultiTool
{
    public class CustomPhraseManager : HotPhraseManagerForWinForms
    {
        public void OnExpandToNameOfTrigger(object sender, PhraseEventArguments e)
        {
            var customKeySequence = (CustomKeySequence) e.State.KeySequence;
            SendBackspaces(customKeySequence.BackspaceCount);
            var textToSend = e.Name;
            if (e.State.KeySequence.WildcardMatchType != WildcardMatchType.None
                && e.State.MatchResult.Value.IsNotEmpty())
            {
                var templateToFind = WildcardTemplate(e.State.KeySequence);
                textToSend = textToSend.Replace(templateToFind, e.State.MatchResult.Value);
            }
            SendString(textToSend);
        }

        private string WildcardTemplate(KeySequence stateKeySequence)
        {
            return "~~~~"; // new string('*', stateKeySequence.WildcardCount);
        }

        public void AddOrReplace(string name, int backspaceCount, params PKey[] keys)
        {
            var  customKeySequence = new CustomKeySequence(name, keys.ToList(), OnExpandToNameOfTrigger, backspaceCount);
            Keyboard.AddOrReplace(customKeySequence);
        }

        public void AddFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            InsideQuotedEntry = false;
            AddSet(File.ReadAllText(path));
        }

        public bool InsideQuotedEntry { get; set; }
        public string CurrentEntry { get; set; }

        private IEnumerable<string> ReduceAndExpand(string arg)
        {
            var list = new List<string>();
            arg = ReduceIndentation(arg);

            if(arg.Contains(" = \""))
            {
                InsideQuotedEntry = true;
                CurrentEntry = arg.TokensAfterFirst(" = \"");
            }
            if(arg.Contains(" = "))
            {

            }
            return list;
        }
        
        public string ReduceIndentation(string line)
        {
            while(line.StartsWith(" "))
            {
                line = line.Substring(1);
            }
            return line;
        }

        public CustomKeySequence AddOrReplace(string keys)
        {
            var pKeyList = ToPKeyList(keys, null, out var wildcardMatchType, out var wildcardCount);
            var keySequence =
                new CustomKeySequence(keys, pKeyList, OnExpandToNameOfTrigger, ToBackspaceCount(pKeyList));
            if (wildcardCount <= 0) return keySequence;

            keySequence.WildcardMatchType = wildcardMatchType;
            keySequence.WildcardCount = wildcardCount;
            return keySequence;
        }

        public List<CustomKeySequence> AddSet(string text)
        {
            var result = new List<CustomKeySequence>();
            var whens = text
                .Replace("\r", "")
                .AllTokens("When ", StringSplitOptions
                    .RemoveEmptyEntries)
                .Where(t => t.Replace("\n","").Trim().IsNotEmpty())
                .ToList();

            foreach(var when in whens)
            {
                var ors = when
                    .Replace("\nOr ", "Or ", StringComparison
                        .InvariantCultureIgnoreCase)
                    .AllTokens("Or ", StringSplitOptions
                        .RemoveEmptyEntries);

                for (var i = 0; i < ors.Count; i++)
                {
                    if (ors[i].TokenCount("\n") == 2
                        && ors[i].EndsWith("\n"))
                    {
                        ors[i] = ors[i].FirstToken("\n");
                    }
                }

                var whenKeysText = ors[0].FirstToken(" type ");
                var whenKeys = ToPKeyList(whenKeysText, null, out var wildcardMatchType, out var wildcardCount);

                if (whenKeys.IsEmpty()) continue;

                var whenExpansion = ors[0].TokensAfterFirst(" type ");
                if (whenExpansion.IsNotEmpty())
                {
                    var backspaceCount = ToBackspaceCount(whenKeys);
                    var keySequence = new CustomKeySequence(whenExpansion, whenKeys, OnExpandToNameOfTrigger, backspaceCount);
                    keySequence.WildcardMatchType = wildcardMatchType;
                    keySequence.WildcardCount = wildcardCount;
                    result.Add(keySequence);
                }

                var prepend = new List<PKey>(whenKeys.GetRange(0, whenKeys.Count - 1));
                for (var index = 1; index < ors.Count; index++)
                {
                    var @or = ors[index];
                    var name = @or.FirstToken(" type ");
                    var keys = ToPKeyList(name, prepend, out wildcardMatchType, out wildcardCount);
                    if (keys.IsEmpty()) continue;

                    var expansion = @or.TokensAfterFirst(" type ");
                    if (expansion.IsEmpty()) continue;

                    var backspaceCount = ToBackspaceCount(keys);
                    var keySequence = new CustomKeySequence(expansion, keys, OnExpandToNameOfTrigger, backspaceCount);
                    keySequence.WildcardMatchType = wildcardMatchType;
                    keySequence.WildcardCount = wildcardCount;
                    result.Add(keySequence);
                }
            }   
            return result;
        }

        private int ToBackspaceCount(List<PKey> pKeyList)
        {
            var count = pKeyList.Count(key => (key is >= PKey.D0 and <= PKey.Z or >= PKey.NumPad0 and <= PKey.NumPad9));
            return count;
        }

        public static List<PKey> ToPKeyList(string keys, List<PKey> prepend, out WildcardMatchType wildcardMatchType, out int wildcardCount)
        {
            var pKeyList = prepend.IsEmpty()
                ? new List<PKey>()
                : new List<PKey>(prepend);

            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (keys.IsEmpty())
            {
                return pKeyList;
            }

            var keyParts = keys.AllTokens();
            foreach (var keyPart in keyParts)
            {
                var pKey = FromString(keyPart, out WildcardMatchType wildcardMatchTypeInner, out int wildcardCountInner);
                
                if(wildcardCountInner < 1)
                {
                    pKeyList.Add(pKey);
                }
                else
                {
                    wildcardMatchType = wildcardMatchTypeInner;
                    wildcardCount = wildcardCountInner;
                }
            }
            

            return pKeyList;
        }

        public static PKey FromString(string singlePKeyText, out WildcardMatchType wildcardMatchType, out int wildcardCount)
        {
            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (singlePKeyText.IsEmpty())
            {
                return PKey.None;
            }

            if (singlePKeyText.Length > 0)
            {
                if (singlePKeyText.All(x => x == '#'))
                {
                    wildcardMatchType = WildcardMatchType.Digits;
                    wildcardCount = singlePKeyText.Length;
                    return PKey.None;
                }

                if (singlePKeyText.All(x => x == '*'))
                {
                    wildcardMatchType = WildcardMatchType.AlphaNumeric;
                    wildcardCount = singlePKeyText.Length;
                    return PKey.None;
                }
            }

            switch (singlePKeyText.ToUpper())
            {
                case "0": return PKey.D0;
                case "1": return PKey.D1;
                case "2": return PKey.D2;
                case "3": return PKey.D3;
                case "4": return PKey.D4;
                case "5": return PKey.D5;
                case "6": return PKey.D6;
                case "7": return PKey.D7;
                case "8": return PKey.D8;
                case "9": return PKey.D9;
            }

            if(Enum.TryParse(typeof(PKey), singlePKeyText, true, out object pKey))
            {
                if (pKey != null) 
                    return (PKey) pKey;
            }

            return PKey.None;
        }
    }
}