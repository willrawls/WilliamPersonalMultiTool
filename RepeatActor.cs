namespace WilliamPersonalMultiTool
{
    public class RepeatActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public RepeatActor(string item) : base(ActionType.Repeat, item)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }
}