using System;
using TrueCraft.API.Logic;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
    public class CompassItem : ToolItem
    {
        public static readonly short ItemID = 0x159;

        public override short ID { get { return 0x159; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(6, 3);
        }

        public override string DisplayName { get { return "Compass"; } }
    }
}