﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using WilliamPersonalMultiTool.Properties;
using Win32Interop.Enums;

namespace WilliamPersonalMultiTool
{
    public partial class MainForm : Form
    {
        public CustomPhraseManager Manager { get; set; }
        public List<CustomKeySequence> StaticSequences { get; set; }
        public WindowWorker WindowWorker { get; set; }

        public bool HideStaticSequences;

        public MainForm()
        {
            Manager = new CustomPhraseManager(this);
            InitializeComponent();

            BuildStaticSequences();

            SetupHotPhrases();
            UpdateListView();

            WindowState = Debugger.IsAttached
                ? FormWindowState.Normal
                : FormWindowState.Minimized;
        }

        private void BuildStaticSequences()
        {
            StaticSequences = new List<CustomKeySequence>()
            {
                new("Reload sequences", new List<PKey> {PKey.RControlKey, PKey.RControlKey, PKey.RShiftKey, PKey.RShiftKey}, OnReloadKeySequences, 0),
                new("Edit sequences", new List<PKey> {PKey.RControlKey, PKey.RControlKey, PKey.Alt, PKey.Alt}, OnEditKeySequences, 0),
                new("Turn off all sequences", new List<PKey> {PKey.RControlKey, PKey.Shift, PKey.Alt, PKey.RControlKey}, OnToggleOnOff, 0),
            
                new("Generate a GUID, style N", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.G, PKey.N}, OnGenerateGuid_N, 2),
                new("Generate a GUID, style P", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.G, PKey.P}, OnGenerateGuid_P, 2),

                new("Base64 Encode Clipboard", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.B, PKey.E}, OnEncodeClipboard, 2),
                new("Base64 Decode Clipboard", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.B, PKey.D}, OnDecodeClipboard, 2),
            };
            StaticSequences.ForEach(s => s.BackColor = Color.CadetBlue);

            WindowWorker = new WindowWorker(Manager, Handle);
            StaticSequences.AddRange(WindowWorker.Sequences);
        }

        public string Encode(string plainText) 
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return "";
        }

        public string Decode(string base64EncodedData) 
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return "";
        }



        private void OnToggleOnOff(object sender, PhraseEventArguments e)
        {
            ToggleOnOffButton_Click(null, null);
        }

        private void OnEncodeClipboard(object sender, PhraseEventArguments e)
        {
            var text = Clipboard.GetText();
            if (text.IsEmpty())
                return;

            var encodedText = Encode(text);

            Manager.SendBackspaces(2);
            Manager.SendString(encodedText, 2, true);
        }

        private void OnDecodeClipboard(object sender, PhraseEventArguments e)
        {
            var encodedText = Clipboard.GetText();
            if (encodedText.IsEmpty())
                return;

            var text= Decode(encodedText);

            Manager.SendBackspaces(2);
            Manager.SendString(text, 2, true);
        }

        private void OnGenerateGuid_N(object sender, PhraseEventArguments e)
        {
            var text = Guid.NewGuid().ToString("N");
            Manager.SendBackspaces(2);
            Manager.SendString(text, 2, true);
        }

        private void OnGenerateGuid_P(object sender, PhraseEventArguments e)
        {
            var text = Guid.NewGuid().ToString("B");
            Manager.SendBackspaces(2);
            Manager.SendString(text, 2, true);
        }

        private void OnEditKeySequences(object sender, PhraseEventArguments e)
        {
            EditButton_Click(null, null);
        }

        private void OnReloadKeySequences(object sender, PhraseEventArguments e)
        {
            ReloadButton_Click(null, null);
        }

        public void SetupHotPhrases()
        {
            var path = WpmtPath();
            
            Manager.Keyboard.KeySequences.Clear();
            
            foreach(var staticSequence in StaticSequences)
                Manager.Keyboard.AddOrReplace(staticSequence);

            Manager.AddFromFile(path);
        }

        private static string WpmtPath(string filename = "Default")
        {
            var appDataXlg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XLG");
            if (!Directory.Exists(appDataXlg)) Directory.CreateDirectory(appDataXlg);

            var path = Path.Combine(appDataXlg, $"{filename}.wpmt");
            return path;
        }

        private void UpdateListView()
        {
            KeySequenceList.Items.Clear();
            foreach (var keySequence in Manager.Keyboard.KeySequences)
            {
                var customKeySequence = (CustomKeySequence) keySequence;
                var keys = "";

                if (HideStaticSequences && customKeySequence.BackColor != Color.White) continue;

                for (var index = 0; index < customKeySequence.Sequence.Count; index++)
                {
                    var key = customKeySequence.Sequence[index];
                    var comma = index == customKeySequence.Sequence.Count - 1 ? "" : ", ";
                    keys += $"{key}{comma}";
                }

                if (customKeySequence.WildcardCount > 0)
                {
                    char matchType;
                    switch (customKeySequence.WildcardMatchType)
                    {
                        case WildcardMatchType.Anything:
                        case WildcardMatchType.AlphaNumeric:
                        case WildcardMatchType.NotAlphaNumeric:
                            matchType = '?';
                            break;

                        case WildcardMatchType.Letters:
                            matchType = '*';
                            break;

                        case WildcardMatchType.Digits:
                            matchType = '#';
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    keys += $", {new string(matchType, customKeySequence.WildcardCount)}";
                }

                var listViewItem = new ListViewItem
                {
                    Text = keys, 
                    BackColor = customKeySequence.BackColor
                };

                var listViewSubItem = new ListViewItem.ListViewSubItem(listViewItem, customKeySequence.Name);
                listViewItem.SubItems.Add(listViewSubItem);
                KeySequenceList.Items.Add(listViewItem);
            }

            KeySequenceList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void EditButton_Click(object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo("notepad.exe", $"\"{WpmtPath()}\"");
            var notepad = Process.Start(startInfo);
        }

        public void ReloadButton_Click(object sender, EventArgs e)
        {
            SetupHotPhrases();
            UpdateListView();
        }

        private void ToggleOnOffButton_Click(object sender, EventArgs e)
        {
            if(ToggleOnOffButton.Text == "Turn &Off")
            {
                Manager.Keyboard.KeySequences.Clear();
                KeySequenceList.Items.Clear();
            }
            else
            {
                ReloadButton_Click(null, null);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWindowPosition();
        }

        private void RestoreWindowPosition()
        {
            if (Settings.Default.HasSetDefaults)
            {
                WindowState = Settings.Default.WindowState;
                Location = Settings.Default.Location;
                Size = Settings.Default.Size;
            }
        }

        private void SaveWindowPosition()
        {
            Settings.Default.WindowState = WindowState;

            if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.Location = Location;
                Settings.Default.Size = Size;
            }
            else
            {
                Settings.Default.Location = RestoreBounds.Location;
                Settings.Default.Size = RestoreBounds.Size;
            }

            Settings.Default.HasSetDefaults = true;

            Settings.Default.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RestoreWindowPosition();
        }

        private void HideStaticSequencesButton_Click(object sender, EventArgs e)
        {
            HideStaticSequences = !HideStaticSequences;
            UpdateListView();
        }

    }
}