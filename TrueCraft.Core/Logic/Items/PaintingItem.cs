using System;

namespace TrueCraft.Core.Logic.Items
{
    public class PaintingItem : ItemProvider
    {
        public static readonly short ItemID = 0x141;

        public override short ID { get { return 0x141; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(10, 1);
        }

        public override string DisplayName { get { return "Painting"; } }
    }
}