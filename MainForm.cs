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
            Manager.AddOrReplace("william.rawls@gmail.com", 3, PKey.CapsLock, PKey.W, PKey.R, PKey.G);
            Manager.AddOrReplace("willrawls@hotmail.com", 1, PKey.CapsLock, PKey.CapsLock, PKey.H);
            Manager.AddOrReplace("2279 Perez Street Apt 290", 3, PKey.CapsLock, PKey.W, PKey.R, PKey.A);
            Manager.AddFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WR1.wpmt"));
        }
    }
}
