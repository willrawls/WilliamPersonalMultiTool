using System;
using System.Collections.Immutable;
using System.IO;
using System.Windows.Forms;
using NHotPhrase.Keyboard;

namespace WilliamPersonalMultiTool
{

    public partial class MainForm : Form
    {
        public CustomPhraseManager Manager { get; set; } = new();
        public MainForm()
        {
            SetupHotPhrases();
            WindowState = FormWindowState.Minimized;
        }

        public void SetupHotPhrases()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WR1.wpmt");
            Manager.AddFromFile(path);
        }
    }
}
