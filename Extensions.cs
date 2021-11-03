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
        public static List<PKey> ToPKeyList(
            this string keys,
            List<PKey> prepend,
            out WildcardMatchType wildcardMatchType,
            out int wildcardCount)
        {
            var pKeyList = prepend.IsEmpty()
                ? new List<PKey>()
                : new List<PKey>(prepend);

            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (keys.IsEmpty()) return pKeyList;

            List<string> keyParts;
            if (keys.StartsWith("When")) keyParts = keys[4..].TrimStart().AllTokens();
            else if (keys.StartsWith("Or")) keyParts = keys[2..].TrimStart().AllTokens();
            else keyParts = keys.AllTokens();

            foreach (var keyPart in keyParts)
            {
                List<PKey> additionalKeys = new List<PKey>();
                var pKey = ToPKey(keyPart, out var wildcardMatchTypeInner, out var wildcardCountInner, additionalKeys);

                if (wildcardCountInner < 1)
                {
                    if (pKey != PKey.None)
                        pKeyList.Add(pKey);
                    if (additionalKeys.IsNotEmpty())
                        pKeyList.AddRange(additionalKeys);
                }
                else
                {
                    wildcardMatchType = wildcardMatchTypeInner;
                    wildcardCount = wildcardCountInner;
                }
            }
            return pKeyList;
        }

        public static void ReplaceMatching(this List<KeySequence> keySequences, CustomKeySequence sequence)
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


        public static PKey ToPKey(this string singlePKeyText, out WildcardMatchType wildcardMatchType, out int wildcardCount, List<PKey> additionalKeysFound)
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

            if (Enum.TryParse(typeof(PKey), singlePKeyText, true, out object pKeyObject))
            {
                if (pKeyObject != null)
                {
                    PKey pKey = (PKey)pKeyObject;
                    if (pKey.ToString() == singlePKeyText)
                        return pKey;

                    if (pKey == PKey.CapsLock
                       && (singlePKeyText.ToLower() == "capslock"
                        || singlePKeyText.ToLower() == "captial"))
                        return pKey; // Special case

                    pKey = pKey.FilterDuplicateEnumEntryNames();
                    if (pKey.ToString() == singlePKeyText)
                        return pKey;
                }
            }
            if (singlePKeyText.Length > 0)
            {
                // Treat something like "123" as "1 2 3"
                foreach (var c in singlePKeyText)
                {
                    var key = c.ToString();
                    if (char.IsDigit(c))
                        key = "D" + c;
                    if (!Enum.TryParse(typeof(PKey), key, true, out pKeyObject)) continue;
                    if (pKeyObject == null) continue;

                    var pKey = (PKey)pKeyObject;
                    if (pKey.ToString() == key) 
                        additionalKeysFound?.Add(pKey);
                    else
                    {
                        pKey = pKey.FilterDuplicateEnumEntryNames();
                        if (pKey.ToString() == key) additionalKeysFound?.Add(pKey);
                    }
                }
            }

            return PKey.None;
        }

        public static bool AreEqual(this List<PKey> expected, List<PKey> actual)
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