using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;

namespace WilliamPersonalMultiTool
{
    public partial class MainForm : Form
    {
        public CustomPhraseManager Manager { get; set; } = new();

        public CustomKeySequence ReloadKeySequences { get; set; }
        public CustomKeySequence EditKeySequences { get; set; }

        public MainForm()
        {
            InitializeComponent();

            ReloadKeySequences = new CustomKeySequence("Reload key sequences",
                new List<PKey> { PKey.RControlKey, PKey.RControlKey, PKey.RShiftKey, PKey.RShiftKey },
                (sender, arguments) => { ReloadButton_Click(null, null); }, 0);

            EditKeySequences = new CustomKeySequence("Edit key sequences",
                new List<PKey> { PKey.RControlKey, PKey.RControlKey, PKey.Alt, PKey.Alt },
                (sender, arguments) => { EditButton_Click(null, null); }, 0);

            SetupHotPhrases();
            UpdateListView();

            WindowState = Debugger.IsAttached
                ? FormWindowState.Normal
                : FormWindowState.Minimized;
        }

        public void SetupHotPhrases()
        {
            var path = WpmtPath();
            Manager.Keyboard.KeySequences.Clear();
            Manager.Keyboard.AddOrReplace(ReloadKeySequences);
            Manager.Keyboard.AddOrReplace(EditKeySequences);
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

        private void EditButton_Click(object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo("notepad.exe", $"\"{WpmtPath()}\"");
            var notepad = Process.Start(startInfo);
        }

        private void ReloadButton_Click(object sender, EventArgs e)
        {
            SetupHotPhrases();
            UpdateListView();
        }
    }
}