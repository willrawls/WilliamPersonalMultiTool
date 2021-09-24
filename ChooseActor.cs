namespace WilliamPersonalMultiTool
{
    public class ChooseActor : BaseActor
    {
        public ChooseActor(string item) : base(ActionType.Choose, item)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }
}