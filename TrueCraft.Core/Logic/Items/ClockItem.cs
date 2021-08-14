using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class ClockItem : ToolItem
    {
        public static readonly short ItemID = 0x15B;

        public override short ID { get { return 0x15B; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(6, 4);
        }

        public override string DisplayName { get { return "Clock"; } }
    }
}