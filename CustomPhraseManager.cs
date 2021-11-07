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

            PausableNormalSendKeysAndWait(textToSend);
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
            Keyboard.KeySequences.ReplaceMatching(keySequence);
            return keySequence;
        }

        public CustomKeySequence AddOrReplace(string keys)
        {
            var pKeyList = keys.ToPKeyList(null, out var wildcardMatchType, out var wildcardCount);
            var keySequence =
                new CustomKeySequence(keys, pKeyList, OnExpandToNameOfTrigger, ToBackspaceCount(pKeyList));

            Keyboard.KeySequences.ReplaceMatching(keySequence);

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

            var linesWithNoComments = text
                .Replace("\r", "")
                .LineList()
                .Where(line => !line.Trim().StartsWith("//"))
                .ToList();

            var keySequencesToAdd = new List<KeySequence>();
            Actors ??= new BaseActorList();
            BaseActor actor = null;

            foreach (var line in linesWithNoComments)
            {
                var actionableItem = ActorHelper.GetActionType(line);
                if (actionableItem.ActionableType == ActionableType.Unknown)
                {
                    throw new Exception($"Invalid Line: {line}");
                }
                if (actionableItem.ActionableType == ActionableType.Continuation && actor != null)
                {
                    var continueWith = actor.OnContinue(line);
                    Actors.Add(continueWith);
                    keySequencesToAdd.Add(continueWith.KeySequence);
                }
                else
                {
                    actor = actionableItem.ToActor(line, null);
                    Actors.Add(actor);
                    keySequencesToAdd.Add(actor.KeySequence);
                }
            }

            Actors.KeySequences ??= new KeySequenceList();
            Actors.KeySequences.AddRange(keySequencesToAdd);

            Keyboard.KeySequences ??= new KeySequenceList();
            Keyboard.KeySequences.AddRange(keySequencesToAdd);

            return keySequencesToAdd;
        }

        public BaseActorList Actors { get; set; }

        private void InternalAddGenericAction(BaseActor actor, string parameters, List<PKey> keySequence,
            List<KeySequence> resultingSequences, int wildcardCount, WildcardMatchType wildcardMatchType)
        {
            var action = actor.ActionableType.ToString();
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
            result.ReplaceMatching(orSequence);
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
            var choices = expansion.LineList(StringSplitOptions.TrimEntries).Where(l => l.Length > 0).ToList();
            var choiceList = new List<CustomKeySequenceChoice>();
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

            result.ReplaceMatching(chooseSequence);
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
            PausableNormalSendKeysAndWait(keySequenceChoice.Text);

            //var textToSend = MakeReadyForSending(keySequenceChoice.Text, SplitLength, true);
            //SendStrings(textToSend, 2);
        }

        public static void PausableNormalSendKeysAndWait(string toSend)
        {
            if (toSend.ToLower().Contains("{pause "))
            {
                var parts = toSend.AllTokens("{pause ");
                if (parts.Count == 1)
                    NormalSendKeysAndWait(toSend);
            }
            else
            {
                NormalSendKeysAndWait(toSend);
            }
        }

        public static void NormalSendKeysAndWait(string toSend)
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
            resultingSequences.ReplaceMatching(sequence);
        }

        public int ToBackspaceCount(List<PKey> pKeyList)
        {
            var count = pKeyList.Count(key => key is >= PKey.D0 and <= PKey.Z or >= PKey.NumPad0 and <= PKey.NumPad9);
            return count;
        }

        
        public static KeySequence Factory(string name = null, string keys = null)
        {
            var keySequence = new KeySequence()
            {
                Name = name ?? Guid.NewGuid().ToString(),
            };
            if (keys.IsEmpty()) return keySequence;

            keySequence.Sequence = keys.ToPKeyList(null, out var wildcardMatchType, out var wildcardCount);
            keySequence.WildcardCount = wildcardCount;
            keySequence.WildcardMatchType = wildcardMatchType;
            return keySequence;
        }

    }
}