using System;

namespace TrueCraft.Core.Logic.Items
{
    public class BreadItem : FoodItem
    {
        public static readonly short ItemID = 0x129;

        public override short ID { get { return 0x129; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(9, 2);
        }

        public override float Restores { get { return 2.5f; } }

        public override string GetDisplayName(short metadata)
        {
            return "Bread";
        }
    }
}