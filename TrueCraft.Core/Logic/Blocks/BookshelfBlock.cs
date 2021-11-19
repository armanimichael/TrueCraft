using System;

namespace TrueCraft.Core.Logic.Blocks
{
    public class BookshelfBlock : BlockProvider, IBurnableItem
    {
        public static readonly byte BlockID = 0x2F;
        
        public override byte ID { get { return 0x2F; } }
        
        public override double BlastResistance { get { return 7.5; } }

        public override double Hardness { get { return 1.5; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Bookshelf"; } }

        public TimeSpan BurnTime { get { return TimeSpan.FromSeconds(15); } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Wood;
            }
        }

        public override bool Flammable { get { return true; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(3, 2);
        }
    }
}