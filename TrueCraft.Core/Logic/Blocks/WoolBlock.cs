using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WoolBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x23;
        
        public override byte ID { get { return 0x23; } }
        
        public override double BlastResistance { get { return 4; } }

        public override double Hardness { get { return 0.8; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Wool"; } }

        public override bool Flammable { get { return true; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Cloth;
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 4);
        }
    }
}