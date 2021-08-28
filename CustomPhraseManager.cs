using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
            SendBackspaces(customKeySequence.BackspaceCount, 2);
            var textToSend = e.Name;
            if (e.State.KeySequence.WildcardMatchType != WildcardMatchType.None
                && e.State.MatchResult.Value.IsNotEmpty())
            {
                var templateToFind = WildcardTemplate(e.State.KeySequence);
                textToSend = textToSend.Replace(templateToFind, e.State.MatchResult.Value);
            }

            textToSend = textToSend
                    .Replace(@"\r", "\r")
                    .Replace(@"\n", "\n")
                    .Replace(@"\t", "\t")
                    .Replace(@"\*", @"*")
                    .Replace(@"\\", @"\")
                ;

            var debug = MakeReadyForSending(textToSend, SplitLength, true);
            SendString(textToSend, 2, true);
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
            InsideQuotedEntry = false;
            if (!File.Exists(path))
                return;

            AddSet(File.ReadAllText(path));
        }

        public bool InsideQuotedEntry { get; set; }
        public string CurrentEntry { get; set; }

        public CustomKeySequence Add(CustomKeySequence keySequence)
        {
            Keyboard.KeySequences.Add(keySequence);
            return keySequence;
        }

        public CustomKeySequence AddOrReplace(string keys)
        {
            var pKeyList = ToPKeyList(keys, null, out var wildcardMatchType, out var wildcardCount);
            var keySequence =
                new CustomKeySequence(keys, pKeyList, OnExpandToNameOfTrigger, ToBackspaceCount(pKeyList));
            Keyboard.KeySequences.Add(keySequence);
            if (wildcardCount <= 0) return keySequence;

            keySequence.WildcardMatchType = wildcardMatchType;
            keySequence.WildcardCount = wildcardCount;
            return keySequence;
        }

        public List<CustomKeySequence> AddSet(string text)
        {
            text = text.Replace("\r", "");
            while (text.StartsWith("\n")) text = text.Substring(1);
            while (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);

            var keySequencesToAdd = new List<CustomKeySequence>();
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
                    if (ors[i].TokenCount("\n") == 2 && ors[i].EndsWith("\n"))
                    {
                        ors[i] = ors[i].FirstToken("\n");
                    }
                }

                var actionSeparator = GetActionSeparator(ors[0]);
                var paddedActionSeparator = $" {actionSeparator} ";
                var whenKeysText = ors[0].FirstToken(paddedActionSeparator);
                var parameters = ors[0].TokensAfterFirst(paddedActionSeparator);
                var keysAtWhenLevel = ToPKeyList(whenKeysText, null, out var wildcardMatchType, out var wildcardCount);

                if (keysAtWhenLevel.IsEmpty()) continue;
                InternalAddGenericAction(actionSeparator, parameters, keysAtWhenLevel, keySequencesToAdd, wildcardCount, wildcardMatchType);

                foreach(var @or in ors)
                {
                    actionSeparator = GetActionSeparator(@or);
                    paddedActionSeparator = $" {actionSeparator} ";
                    parameters = @or.TokensAfterFirst(paddedActionSeparator);

                    var keyToPrepend = new List<PKey>(keysAtWhenLevel.GetRange(0, keysAtWhenLevel.Count - 1));
                    var keySequenceForThisOr = ToPKeyList(whenKeysText, keyToPrepend, out wildcardMatchType, out wildcardCount);
                    if (keySequenceForThisOr.IsEmpty()) continue;

                    var expansion = @or.TokensAfterFirst(" " + actionSeparator);
                    if (expansion.IsEmpty()) continue;
                    InternalAddGenericAction(actionSeparator, parameters, keySequenceForThisOr, keySequencesToAdd, wildcardCount, wildcardMatchType);
                }
            }   
            Keyboard.KeySequences.AddRange(keySequencesToAdd);
            return keySequencesToAdd;
        }

        private void InternalAddGenericAction(string action, string parameters, List<PKey> keySequence, List<CustomKeySequence> resultingSequences, int wildcardCount, WildcardMatchType wildcardMatchType)
        {
            if (action == "choose" && parameters.Replace("\r", "").StartsWith("\n"))
                parameters = parameters.Substring(1);

            if (parameters.IsNotEmpty())
            {
                if (action == "run")
                    InternalAddRunTrigger(parameters, keySequence, resultingSequences);
                else if (action == "choose")
                    InternalAddChooseTrigger(parameters, keySequence, resultingSequences);
                else // type
                    InternalAddOrReplace(keySequence, wildcardCount, parameters, wildcardMatchType, resultingSequences);
            }
        }

        public static string[] ActionSeparators = new[] { "type", "run", "choose" };

        public static string GetActionSeparator(string @or)
        {
            var lower = @or.Replace("\r","\n").ToLower();
            foreach (var separator in ActionSeparators)
            {
                if (lower.Contains($" {separator} ")
                    || lower.Contains($" {separator}\n"))
                {
                    return separator;
                }
            }
            return " unknown ";
        }

        private void InternalAddOrReplace(List<PKey> keys, int wildcardCount, string expansion, WildcardMatchType wildcardMatchType, List<CustomKeySequence> resultingSequences)
        {
            var backspaceCount = ToBackspaceCount(keys) + wildcardCount;
            var keySequence = new CustomKeySequence(expansion, keys, OnExpandToNameOfTrigger, backspaceCount)
                {
                    WildcardMatchType = wildcardMatchType,
                    WildcardCount = wildcardCount
                };
            InternalAddOrReplace(keySequence, resultingSequences);
        }

        private void InternalAddRunTrigger(string expansion, List<PKey> keys, List<CustomKeySequence> result)
        {
            var executablePath = "";
            var arguments = "";
            var runName = "";

            if (expansion.Contains("\""))
            {
                executablePath = expansion.TokenAt(2, "\"");
                arguments = expansion.TokensAfter(2, "\"").Trim();

                if (arguments.StartsWith("\"") && arguments.EndsWith("\""))
                {
                    arguments = arguments.Substring(1, arguments.Length - 2);
                }

                runName = "Run " + $"\"{executablePath}\" " + arguments;
            }
            else
            {
                executablePath = expansion;
                runName = "Run " + expansion;
            }

            var backspaceCount = ToBackspaceCount(keys);
            var orSequence = new CustomKeySequence(runName, keys, OnRunTriggerHandler, backspaceCount)
            {
                ExecutablePath = executablePath,
                Arguments = arguments
            };
            result.Add(orSequence);
        }

        private void OnRunTriggerHandler(object sender, PhraseEventArguments e)
        {
            var customKeySequence = ((CustomKeySequence) e.State.KeySequence);
            if(customKeySequence.BackspaceCount > 0)
                SendBackspaces(customKeySequence.BackspaceCount);

            Process.Start(customKeySequence.ExecutablePath, customKeySequence.Arguments);
        }

        private void InternalAddChooseTrigger(string expansion, List<PKey> keys, List<CustomKeySequence> result)
        {
            List<string> choices = expansion.LineList(StringSplitOptions.TrimEntries).Where(l => l.Length > 0).ToList();
            List<CustomKeySequenceChoice> choiceList = new List<CustomKeySequenceChoice>();
            foreach(var choice in choices)
            {
                choiceList.Add(new CustomKeySequenceChoice
                {
                    Text = choice.TrimStart(),
                });
            }

            var backspaceCount = ToBackspaceCount(keys);
            var name = keys.Aggregate("Choose ", (current, key) => current + $"{key.ToSendKeysText()} ").Trim();

            var chooseSequence = new CustomKeySequence(name, keys, OnChooseTriggerHandler, backspaceCount)
            {
                Choices = new List<CustomKeySequenceChoice>(choiceList),
            };
            result.Add(chooseSequence);
        }

        private void OnChooseTriggerHandler(object sender, PhraseEventArguments e)
        {
            var customKeySequence = ((CustomKeySequence) e.State.KeySequence);
            if(customKeySequence.BackspaceCount > 0)
                SendBackspaces(customKeySequence.BackspaceCount);

            var choice = e.State.MatchResult?.ValueAsInt();
            if (choice is not >= 0 || choice.Value > customKeySequence.Choices.Count) return;

            var keySequenceChoice = customKeySequence.Choices[choice.Value];
            var textToSend = MakeReadyForSending(keySequenceChoice.Text, SplitLength, true);
            SendStrings(textToSend, 2);
        }

        private void InternalAddOrReplace(CustomKeySequence sequence, List<CustomKeySequence> resultingSequences)
        {
            for (var index = 0; index < Keyboard.KeySequences.Count; index++)
            {
                var ks1 = Keyboard.KeySequences[index];
                if (!AreEqual(ks1.Sequence, sequence.Sequence)) 
                    continue;

                Keyboard.KeySequences.RemoveAt(index);
            }

            for (var index = 0; index < resultingSequences.Count; index++)
            {
                var ks1 = resultingSequences[index];
                if (!AreEqual(ks1.Sequence, sequence.Sequence)) 
                    continue;

                resultingSequences.RemoveAt(index);
            }

            resultingSequences.Add(sequence);
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
                var pKey = FromString(keyPart, out var wildcardMatchTypeInner, out var wildcardCountInner);
                
                if(wildcardCountInner < 1)
                {
                    if(pKey != PKey.None)
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

        public static bool AreEqual(List<PKey> expected, List<PKey> actual)
        {
            if (expected == null || actual == null)
                return false;

            if(expected.Count != actual.Count)
                return false;

            for (var i = 0; i < expected.Count; i++)
            {
                if (actual[i] != expected[i])
                    return false;
            }

            return true;
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