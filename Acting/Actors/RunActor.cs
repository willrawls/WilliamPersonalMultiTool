using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MetX.Standard.IO;
using MetX.Standard.Library;
using NHotPhrase.Phrase;
using WilliamPersonalMultiTool.Custom;

namespace WilliamPersonalMultiTool.Acting.Actors
{
    public class RunActor : BaseActor
    {
        public string TargetExecutable { get; set; }

        public Verb Normal { get; set; }
        public Verb Maximize { get; set; }
        public Verb Minimize { get; set; }

        public string WorkingFolder { get; set; }
        public string Filename { get; set; }

        public RunActor()
        {
            ActionableType = ActionableType.Run;

            Minimize = AddLegalVerb("minimize");
            Maximize = AddLegalVerb("maximize");
            Normal = AddLegalVerb("normal");
            OnAct = Act;
            CanContinue = false;
            DefaultVerb = Normal;
        }

        public bool Act(PhraseEventArguments phraseEventArguments)
        {
            ProcessWindowStyle windowStyle;
            if (ExtractedVerbs.Contains(Maximize)) windowStyle = ProcessWindowStyle.Maximized;
            else if (ExtractedVerbs.Contains(Minimize)) windowStyle = ProcessWindowStyle.Minimized;
            else windowStyle = ProcessWindowStyle.Normal;

            if(File.Exists(Filename))
                FileSystem.FireAndForget(Filename, Arguments, WorkingFolder, windowStyle);

            return false;
        }

        public override bool Initialize(string item)
        {
            if (!base.Initialize(item))
                return false;

            if(!KeySequence.Name.StartsWith("Run "))
                KeySequence.Name = "Run " + KeySequence.Name;

            var argumentWorkspace = Arguments;
            if (argumentWorkspace.Trim().Length == 0)
                return false;

            if (argumentWorkspace.StartsWith(" "))
                argumentWorkspace = argumentWorkspace.Substring(1);

            if (!argumentWorkspace.Contains("\""))
            {
                Arguments = argumentWorkspace;
                return true;
            }

            TargetExecutable = argumentWorkspace.TokenAt(2, "\"");
            argumentWorkspace = argumentWorkspace.TokensAfter(2, "\"");

            if (argumentWorkspace.StartsWith(" "))
                argumentWorkspace = argumentWorkspace.Substring(1);

            if (argumentWorkspace.Contains("\""))
            {
                Filename = argumentWorkspace.TokenAt(1, "\"");
                argumentWorkspace = argumentWorkspace.TokenAt(2, "\"");
            }

            var customKeySequence = (CustomKeySequence)KeySequence;
            customKeySequence.ExecutablePath = TargetExecutable;
            Arguments = argumentWorkspace;
            if (Filename.Trim().Length > 0)
            {
                Filename = Filename.Trim();
                customKeySequence.Arguments = $"\"{Filename}\" \"{Arguments}\"";
            }
            else
                customKeySequence.Arguments = $"\"{Arguments}\"";

            if(!customKeySequence.Name.StartsWith("Run "))
                customKeySequence.Name = "Run " + customKeySequence.Name;
            return true;
        }
    }
}