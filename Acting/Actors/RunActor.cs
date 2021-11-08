using System.Diagnostics;
using System.IO;
using MetX.Standard.IO;
using MetX.Standard.Library;
using NHotPhrase.Phrase;

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

            if (Arguments.Trim().Length == 0)
                return false;

            if (Arguments.Contains("\""))
            {
                Filename = Arguments.TokenAt(2, "\"");
                Arguments = Arguments.TokensAfter(2, "\"");

                if (Arguments.Contains("\""))
                {
                    WorkingFolder = Arguments.TokenAt(2, "\"");
                    Arguments = Arguments.TokensAfter(2, "\"");
                }
            }

            return true;
        }
    }
}