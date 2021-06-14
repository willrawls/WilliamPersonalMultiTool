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
            SendString(e.Name);
        }

        public void AddOrReplace(string name, int backspaceCount, params PKey[] keys)
        {
            var  customKeySequence = new CustomKeySequence(name, keys.ToList(), OnExpandToNameOfTrigger, backspaceCount);
            Keyboard.AddOrReplace(customKeySequence);
        }

        public void AddFromText(string text)
        {
            var result = text.LineList(StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(ReduceAndExpand)
                .ToList();

        }

        public void AddFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            InsideQuotedEntry = false;
            AddFromText(File.ReadAllText(path));
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
            var pKeyList = ToPKeyList(null, keys);
            var keySequence =
                new CustomKeySequence(keys, pKeyList, OnExpandToNameOfTrigger, ToBackspaceCount(pKeyList));
            return keySequence;
        }

        private int ToBackspaceCount(List<PKey> pKeyList)
        {
            var count = pKeyList.Count(key => (key is >= PKey.D0 and <= PKey.Z or >= PKey.NumPad0 and <= PKey.NumPad9));
            return count;
        }

        public static List<PKey> ToPKeyList(List<PKey> prepend, string keys)
        {
            var pKeyList = prepend.IsEmpty()
                ? new List<PKey>()
                : new List<PKey>(prepend);

            if (keys.IsEmpty())
                return pKeyList;

            var keyParts = keys.AllTokens();
            pKeyList.AddRange(keyParts.Select(FromString));

            return pKeyList;
        }

        public static PKey FromString(string singlePKeyText)
        {
            if (singlePKeyText.IsEmpty())
                return PKey.None;

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