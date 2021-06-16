using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using WilliamPersonalMultiTool.Properties;

namespace WilliamPersonalMultiTool
{
    public partial class MainForm : Form
    {
        public CustomPhraseManager Manager { get; set; } = new();

        public List<CustomKeySequence> StaticSequences { get; set; }

        public MainForm()
        {
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
            var form = this;
            StaticSequences = new List<CustomKeySequence>()
            {
                new("Reload key sequences", new List<PKey> {PKey.RControlKey, PKey.RControlKey, PKey.RShiftKey, PKey.RShiftKey}, OnReloadKeySequences, 0),
                new("Edit key sequences", new List<PKey> {PKey.RControlKey, PKey.RControlKey, PKey.Alt, PKey.Alt}, OnEditKeySequences, 0),
                new("Toggle all key sequences off", new List<PKey> {PKey.RControlKey, PKey.Shift, PKey.Alt, PKey.RControlKey}, OnToggleOnOff, 0),
                new("Generate GUID style N", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.G, PKey.N}, OnGenerateGuid_N, 2),
                new("Generate GUID style P", new List<PKey> {PKey.CapsLock, PKey.CapsLock, PKey.G, PKey.P}, OnGenerateGuid_P, 2),
            };
        }

        private void OnToggleOnOff(object sender, PhraseEventArguments e)
        {
            ToggleOnOffButton_Click(null, null);
        }

        private void OnGenerateGuid_N(object sender, PhraseEventArguments e)
        {
            var text = Guid.NewGuid().ToString("N");
            Manager.SendBackspaces(2, 2);
            Manager.SendString(text, 2, true);
        }

        private void OnGenerateGuid_P(object sender, PhraseEventArguments e)
        {
            var text = Guid.NewGuid().ToString("P");
            Manager.SendBackspaces(2, 2);
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
                var keys = "";
                for (var index = 0; index < keySequence.Sequence.Count; index++)
                {
                    var key = keySequence.Sequence[index];
                    var comma = index == keySequence.Sequence.Count - 1 ? "" : ", ";
                    keys += $"{key}{comma}";
                }

                if (keySequence.WildcardCount > 0)
                {
                    char matchType;
                    switch (keySequence.WildcardMatchType)
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

                    keys += $", {new string(matchType, keySequence.WildcardCount)}";
                }

                var listViewItem = new ListViewItem { Text = keys };
                var listViewSubItem1 = new ListViewItem.ListViewSubItem(listViewItem, keySequence.Name);
                listViewItem.SubItems.Add(listViewSubItem1);
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
    }
}