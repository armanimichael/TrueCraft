using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Modelling.Blocks;

public class CobwebModeller : FlatQuadModeller
{
    static CobwebModeller()
    {
        RegisterRenderer(CobwebBlock.BlockID, new CobwebModeller());
    }

    protected override Vector2 TextureMap => new(11, 0);
}