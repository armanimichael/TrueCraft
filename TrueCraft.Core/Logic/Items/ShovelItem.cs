﻿using System;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class ShovelItem : ToolItem
    {
        public override ToolType ToolType
        {
            get
            {
                return ToolType.Shovel;
            }
        }
    }

    public class WoodenShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x10D;

        public override short ID { get { return 0x10D; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(0, 5);
        }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Shovel"; } }
    }

    public class StoneShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x111;

        public override short ID { get { return 0x111; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(1, 5);
        }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Shovel"; } }
    }

    public class IronShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x100;

        public override short ID { get { return 0x100; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(2, 5);
        }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Shovel"; } }
    }

    public class GoldenShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x11C;

        public override short ID { get { return 0x11C; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(4, 5);
        }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Shovel"; } }
    }

    public class DiamondShovelItem : ShovelItem
    {
        public static readonly short ItemID = 0x115;

        public override short ID { get { return 0x115; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(3, 5);
        }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override string DisplayName { get { return "Diamond Shovel"; } }
    }
}