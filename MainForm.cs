using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{

    public partial class MainForm : Form
    {
        public CustomPhraseManager Manager { get; set; } = new();
        public MainForm()
        {
            InitializeComponent();
            SetupHotPhrases();
            UpdateListView();
            if(!Debugger.IsAttached)
                WindowState = FormWindowState.Minimized;
        }

        public void SetupHotPhrases()
        {
            var appDataXlg = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XLG");
            if (!Directory.Exists(appDataXlg))
            {
                Directory.CreateDirectory(appDataXlg);
            }
            var path = Path.Combine(appDataXlg, "Default.wpmt");
            Manager.AddFromFile(path);
        }

        private void UpdateListView()
        {
            KeySequenceList.Items.Clear();
            foreach (var keySequence in Manager.Keyboard.KeySequences)
            {
                var keys = "";
                foreach (var key in keySequence.Sequence)
                {
                    keys += $"{key}, ";
                }

                var listViewItem = new ListViewItem {Text = keys};
                var listViewSubItem1 = new ListViewItem.ListViewSubItem(listViewItem, keySequence.Name);
                listViewItem.SubItems.Add(listViewSubItem1);
                KeySequenceList.Items.Add(listViewItem);
            }
        }
    }
}
