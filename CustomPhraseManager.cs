using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetX.Standard.Library;
using MetX.Standard.Library.Extensions;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using NHotPhrase.WindowsForms;

namespace WilliamPersonalMultiTool
{
    public class CustomPhraseManager : HotPhraseManagerForWinForms
    {
        private string _lastFilePath = "";
        public Form Parent { get; }

        public bool InsideQuotedEntry { get; set; }
        public string CurrentEntry { get; set; }

        public CustomPhraseManager(Form parent, string textOfSequencesToAdd = null)
        {
            Parent = parent;
            if (textOfSequencesToAdd.IsNotEmpty())
                AddSet(textOfSequencesToAdd);
        }

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

            textToSend = textToSend
                    .Replace(@"\r", "{RETURN}")
                    .Replace(@"\n", "{RETURN}")
                    .Replace(@"\t", "{TAB}")
                    .Replace(@"\*", @"*")
                    .Replace(@"\\", @"\")
                ;

            // var debug = MakeReadyForSending(textToSend, SplitLength, true);
            // SendString(textToSend, 2, true);

            NormalSendKeysAndWait(textToSend);
        }

        private string WildcardTemplate(KeySequence stateKeySequence)
        {
            return "~~~~"; // new string('*', stateKeySequence.WildcardCount);
        }

        public void AddOrReplace(string name, int backspaceCount, params PKey[] keys)
        {
            var customKeySequence = new CustomKeySequence(name, keys.ToList(), OnExpandToNameOfTrigger, backspaceCount);
            Keyboard.AddOrReplace(customKeySequence);
        }

        public void AddFromFile(string path)
        {
            InsideQuotedEntry = false;
            if (!File.Exists(path))
                return;

            AddSet(File.ReadAllText(path));
        }

        public CustomKeySequence Add(CustomKeySequence keySequence)
        {
            ReplaceMatching(Keyboard.KeySequences, keySequence);
            return keySequence;
        }

        public CustomKeySequence AddOrReplace(string keys)
        {
            List<PKey> pKeyList = ToPKeyList(keys, null, out var wildcardMatchType, out var wildcardCount);
            var keySequence =
                new CustomKeySequence(keys, pKeyList, OnExpandToNameOfTrigger, ToBackspaceCount(pKeyList));

            ReplaceMatching(Keyboard.KeySequences, keySequence);

            if (wildcardCount <= 0) return keySequence;

            keySequence.WildcardMatchType = wildcardMatchType;
            keySequence.WildcardCount = wildcardCount;
            return keySequence;
        }

        public List<KeySequence> AddSet(string text)
        {
            text = text.Replace("\r", "");
            while (text.StartsWith("\n")) text = text.Substring(1);
            while (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);

            List<KeySequence> keySequencesToAdd = new List<KeySequence>();
            var textWithNoComments = string.Join("\n",
                text.Replace("\r", "")
                    .LineList()
                    .Where(line => !line.Trim().StartsWith("//"))
                    .ToList());

            List<string> whens = textWithNoComments
                .AllTokens("When ", StringSplitOptions
                    .RemoveEmptyEntries)
                .Where(t => t.Replace("\n", "").Trim().IsNotEmpty())
                .ToList();

            foreach (var when in whens)
            {
                List<string> ors = when
                    .Replace("\nOr ", "Or ", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("\n  Or ", "Or ", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("\n   Or ", "Or ", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("\n    Or ", "Or ", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("\n\tOr ", "Or ", StringComparison.InvariantCultureIgnoreCase)
                    .AllTokens("Or ", StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < ors.Count; i++)
                    if (ors[i].TokenCount("\n") == 2 && ors[i].EndsWith("\n"))
                        ors[i] = ors[i].FirstToken("\n");

                KeyValuePair<string, ActionType> actionTypeEntry = ActorHelper.GetActionType(ors[0]);
                if (actionTypeEntry.Value == ActionType.Unknown)
                    throw new Exception($"Invalid Entry: {when}");

                var parameters = ors[0].TokensAfterFirst(actionTypeEntry.Key).Substring(1);
                var actor = ActorHelper.Factory(actionTypeEntry.Value, parameters);
                var paddedActionSeparator = $" {actionTypeEntry.Key}";
                var whenKeysText = ors[0].FirstToken(paddedActionSeparator);
                
                List<PKey> keysAtWhenLevel =
                    ToPKeyList(whenKeysText, null, out var wildcardMatchType, out var wildcardCount);

                if (keysAtWhenLevel.IsEmpty()) continue;
                ReplaceMatching(result, orSequence);
                actor.AddAction(parameters, keysAtWhenLevel, keySequencesToAdd, wildcardCount, wildcardMatchType);
                InternalAddGenericAction(actionTypeEntry.Key, parameters, keysAtWhenLevel, keySequencesToAdd, wildcardCount, wildcardMatchType);

                if (ors.Count < 2) continue;

                foreach (var or in ors.Skip(1))
                {
                    actionTypeEntry = ActorHelper.GetActionType(or);
                    if (actionTypeEntry.Value == ActionType.Unknown)
                        throw new Exception($"Invalid Entry: {when}");

                    paddedActionSeparator = $" {actionTypeEntry.Key} ";

                    parameters = ors[0].TokensAfterFirst(paddedActionSeparator).Substring(1);

                    var orKeyText = or.FirstToken(paddedActionSeparator);
                    List<PKey> keysToPrepend = new List<PKey>(keysAtWhenLevel.GetRange(0, keysAtWhenLevel.Count - 1));

                    List<PKey> keySequenceForThisOr = ToPKeyList(orKeyText, keysToPrepend, out wildcardMatchType,
                        out wildcardCount);
                    if (keySequenceForThisOr.IsEmpty()) continue;

                    var expansion = or.TokensAfterFirst(" " + actionTypeEntry);
                    if (expansion.IsEmpty()) continue;
                    InternalAddGenericAction(actor, actionTypeEntry.Key, parameters, keySequenceForThisOr, keySequencesToAdd,
                        wildcardCount, wildcardMatchType);
                }
            }

            Keyboard.KeySequences.AddRange(keySequencesToAdd);
            return keySequencesToAdd;
        }

        private void InternalAddGenericAction(string action, string parameters,
            List<PKey> keySequence, List<KeySequence> resultingSequences, int wildcardCount, WildcardMatchType wildcardMatchType)
        {
            if (action.StartsWith("choose") && parameters.Replace("\r", "").StartsWith("\n"))
                parameters = parameters.Substring(1);

            if (parameters.IsNotEmpty())
            {
                if (action == "run")
                    InternalAddRunTrigger(parameters, keySequence, resultingSequences);
                else if (action.StartsWith("choose"))
                    InternalAddChooseTrigger(action, parameters, keySequence, resultingSequences);
                else // type
                    InternalAddOrReplace(keySequence, wildcardCount, parameters, wildcardMatchType, resultingSequences);
            }
        }

        private void InternalAddOrReplace(List<PKey> keys, int wildcardCount, string expansion,
            WildcardMatchType wildcardMatchType, List<KeySequence> resultingSequences)
        {
            var backspaceCount = ToBackspaceCount(keys) + wildcardCount;
            var keySequence = new CustomKeySequence(expansion, keys, OnExpandToNameOfTrigger, backspaceCount)
            {
                WildcardMatchType = wildcardMatchType,
                WildcardCount = wildcardCount
            };
            InternalAddOrReplace(keySequence, resultingSequences);
        }

        private void InternalAddRunTrigger(string expansion, List<PKey> keys, List<KeySequence> result)
        {
            var executablePath = "";
            var arguments = "";
            var runName = "";

            if (expansion.Contains("\""))
            {
                executablePath = expansion.TokenAt(2, "\"");
                arguments = expansion.TokensAfter(2, "\"").Trim();

                if (arguments.StartsWith("\"") && arguments.EndsWith("\""))
                    arguments = arguments.Substring(1, arguments.Length - 2);

                runName = "Run " + $"\"{executablePath}\" " + arguments;
            }
            else
            {
                executablePath = expansion;
                runName = "Run " +
                          (expansion.StartsWith(" ")
                              ? expansion.TrimStart()
                              : expansion);
            }

            var backspaceCount = ToBackspaceCount(keys);
            var orSequence = new CustomKeySequence(runName, keys, OnRunTriggerHandler, backspaceCount)
            {
                ExecutablePath = executablePath,
                Arguments = arguments,
                BackColor = Color.Tan
            };
            ReplaceMatching(result, orSequence);
        }

        private void OnRunTriggerHandler(object sender, PhraseEventArguments e)
        {
            var customKeySequence = (CustomKeySequence) e.State.KeySequence;
            if (customKeySequence.BackspaceCount > 0)
                SendBackspaces(customKeySequence.BackspaceCount);

            var arguments = customKeySequence.Arguments;
            if (customKeySequence.Arguments.Contains("~"))
            {
                if (arguments.TokenCount("~") > 2)
                {
                    FileDialog dialog = new OpenFileDialog();
                    var filename = arguments.TokenAt(2, "~");
                    dialog.FileName = filename;
                    dialog.RestoreDirectory = true;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    var result = dialog.ShowDialog(Parent);
                    if (result == DialogResult.OK)
                    {
                        _lastFilePath = dialog.FileName;
                        arguments = $"{arguments.FirstToken("~")} {dialog.FileName} {arguments.TokenAt(3, "~")}".Trim();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    FileDialog dialog = new OpenFileDialog();
                    dialog.RestoreDirectory = true;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    if (_lastFilePath.IsNotEmpty()) dialog.FileName = _lastFilePath;

                    var result = dialog.ShowDialog(Parent);
                    if (result == DialogResult.OK)
                    {
                        _lastFilePath = dialog.FileName;
                        arguments = $"{arguments.Replace("~", dialog.FileName)}";
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Process.Start(customKeySequence.ExecutablePath, arguments);
        }

        private void InternalAddChooseTrigger(string action, string expansion, List<PKey> keys,
            List<KeySequence> result)
        {
            List<string> choices = expansion.LineList(StringSplitOptions.TrimEntries).Where(l => l.Length > 0).ToList();
            List<CustomKeySequenceChoice> choiceList = new List<CustomKeySequenceChoice>();
            foreach (var choice in choices)
                choiceList.Add(new CustomKeySequenceChoice
                {
                    Text = choice.TrimStart()
                });

            var backspaceCount = ToBackspaceCount(keys);
            var name = $"Choose from: {expansion}".Replace("\n", "\\n ");

            var chooseSequence = new CustomKeySequence(name, keys, OnChooseTriggerHandler, backspaceCount)
            {
                Choices = new List<CustomKeySequenceChoice>(choiceList),
                WildcardMatchType = WildcardMatchType.Digits,
                WildcardCount = action == "choose2" ? 2 : 1,
                BackColor = Color.PaleGoldenrod
            };

            ReplaceMatching(result, chooseSequence);
        }

        private void OnChooseTriggerHandler(object sender, PhraseEventArguments e)
        {
            var customKeySequence = (CustomKeySequence) e.State.KeySequence;
            if (customKeySequence.BackspaceCount > 0)
            {
                var backspaces = customKeySequence.BackspaceCount + customKeySequence.WildcardCount;
                SendBackspaces(backspaces);
            }

            var choice = e.State.MatchResult?.ValueAsInt();
            if (choice < 1 || choice is not >= 0 || choice.Value > customKeySequence.Choices.Count) return;

            var keySequenceChoice = customKeySequence.Choices[choice.Value - 1];
            NormalSendKeysAndWait(keySequenceChoice.Text);

            //var textToSend = MakeReadyForSending(keySequenceChoice.Text, SplitLength, true);
            //SendStrings(textToSend, 2);
        }

        private static void NormalSendKeysAndWait(string toSend)
        {
            if (toSend.ToLower().Contains("{pause "))
            {
                List<string> parts = toSend.AllTokens("{pause ");
                if (parts.Count == 1)
                    InternalNormalSendKeysAndWait(toSend);
            }
            else
            {
                InternalNormalSendKeysAndWait(toSend);
            }
        }

        private static void InternalNormalSendKeysAndWait(string toSend)
        {
            try
            {
                SendKeys.SendWait(toSend);
            }
            catch
            {
                // Ignored
            }
        }

        private void InternalAddOrReplace(CustomKeySequence sequence, List<KeySequence> resultingSequences)
        {
            ReplaceMatching(resultingSequences, sequence);
        }

        /*
        public static void ReplaceMatching(List<CustomKeySequence> keySequences, CustomKeySequence sequence)
        {
            for (var index = 0; index < keySequences.Count; index++)
            {
                var keySequence = keySequences[index];
                if (!AreEqual(keySequence.Sequence, sequence.Sequence))
                    continue;

                keySequences.RemoveAt(index);
            }
            keySequences.Add(sequence);
        }
        */

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


        private int ToBackspaceCount(List<PKey> pKeyList)
        {
            var count = pKeyList.Count(key => key is >= PKey.D0 and <= PKey.Z or >= PKey.NumPad0 and <= PKey.NumPad9);
            return count;
        }

        public static List<PKey> ToPKeyList(string keys, List<PKey> prepend, out WildcardMatchType wildcardMatchType,
            out int wildcardCount)
        {
            List<PKey> pKeyList = prepend.IsEmpty()
                ? new List<PKey>()
                : new List<PKey>(prepend);

            wildcardMatchType = WildcardMatchType.None;
            wildcardCount = 0;

            if (keys.IsEmpty()) return pKeyList;

            List<string> keyParts = keys.AllTokens();
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
    }
}