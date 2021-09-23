﻿namespace WilliamPersonalMultiTool
{
    public class AdjustActor : BaseActor
    {
        public int RepeatLastCount { get; set; }

        public AdjustActor(string item) : base(ActionType.Adjust, item)
        {
        }

        public override bool Act()
        {
            return true;
        }
    }
}