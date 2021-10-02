namespace WilliamPersonalMultiTool
{
    public class RunActor : BaseActor
    {
        public string TargetExecutable { get; set; }
        public string Arguments { get; set; }

        public RunActor(string item) : base(ActionType.Run, item)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }
}