using System;
using System.Collections.Generic;
using System.Linq;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public static class Extensions
    {
        public static List<PKey> ToPKeyList(string keys, List<PKey> prepend, out WildcardMatchType wildcardMatchType,
            out int wildcardCount)
        {
            var pKeyList = prepend.IsEmpty()
                ? new List<PKey>()
                : new List<PKey>(prepend);

            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (keys.IsEmpty()) return pKeyList;

            var keyParts = keys.AllTokens();
            foreach (var keyPart in keyParts)
            {
                var pKey = FromString(keyPart, out var wildcardMatchTypeInner, out var wildcardCountInner);

                if (wildcardCountInner < 1)
                {
                    if (pKey != PKey.None)
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

        public static void ReplaceMatching(List<KeySequence> keySequences, CustomKeySequence sequence)
        {
            if (keySequences == null || sequence == null)
                return;

            if (keySequences?.Count == 0)
            {
                keySequences.Add(sequence);
                return;
            }

            for (var index = 0; index < keySequences.Count; index++)
            {
                var keySequence = keySequences[index];
                if (!AreEqual(keySequence.Sequence, sequence.Sequence))
                    continue;

                keySequences.RemoveAt(index);
            }

            keySequences.Add(sequence);
        }


        public static PKey FromString(string singlePKeyText, out WildcardMatchType wildcardMatchType,
            out int wildcardCount)
        {
            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (singlePKeyText.IsEmpty()) return PKey.None;

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

            if (Enum.TryParse(typeof(PKey), singlePKeyText, true, out var pKey))
                if (pKey != null)
                    return (PKey) pKey;

            return PKey.None;
        }

        public static bool AreEqual(List<PKey> expected, List<PKey> actual)
        {
            if (expected == null || actual == null)
                return false;

            if (expected.Count != actual.Count)
                return false;

            for (var i = 0; i < expected.Count; i++)
                if (actual[i] != expected[i])
                    return false;

            return true;
        }
    }
}