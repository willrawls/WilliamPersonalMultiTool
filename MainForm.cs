using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetX.Standard.Library;
using NHotPhrase.Keyboard;
using NHotPhrase.Phrase;
using NHotPhrase.WindowsForms;

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

    public class CustomKeySequence : KeySequence
    {
        public int BackspaceCount { get; set; }

        public CustomKeySequence(string name, List<PKey> keys, EventHandler<PhraseEventArguments> hotPhraseEventArgs, int backspaceCount) : base(name, keys, hotPhraseEventArgs)
        {
            if (backspaceCount < 0)
                backspaceCount = 0;
            BackspaceCount = backspaceCount;
        }
    }

    public class CustomPhraseManager : HotPhraseManagerForWinForms
    {
        public void OnExpandToNameOfTrigger(object sender, PhraseEventArguments e)
        {
            var customKeySequence = (CustomKeySequence) e.State.KeySequence;
            SendBackspaces(customKeySequence.BackspaceCount);
            SendString(e.Name);
        }

        public void AddOrReplace(string name, int backspaceCount, params PKey[] keys)
        {
            var  customKeySequence = new CustomKeySequence(name, keys.ToList(), OnExpandToNameOfTrigger, backspaceCount);
            Keyboard.AddOrReplace(customKeySequence);
        }

        public void AddFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            InsideQuotedEntry = false;
            var lines = File.ReadAllLines(path).Where(l => l
                    .IsNotEmpty() && l
                    .Trim() != "*/" && l
                    .Trim().ToLower().StartsWith("/*"))
                .SelectMany(l => l.ReduceAndExpand(l))
                    .ToList();

        }

        public bool InsideQuotedEntry { get; set; }
        public string CurrentEntry { get; set; }

        private IEnumerable<TResult> ReduceAndExpand<TResult>(string arg)
        {
            var list = new List<string>();
            arg = ReduceIndentation(arg);

            if(arg.Contains(" = \""))
            {
                InsideQuotedEntry = true;
                CurrentEntry = arg.TokensAfterFirst(" = \"");
            }
            if(arg.Contains(" = "))
            {

            }

            return (IEnumerable<TResult>) list;
        }


        public string ReduceIndentation(string line)
        {
            while(line.StartsWith(" "))
            {
                line = line.Substring(1);
            }
            return line;
        }
}
}
