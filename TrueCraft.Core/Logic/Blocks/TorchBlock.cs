using System;
using System.Linq;
using TrueCraft.Core.Networking;
using TrueCraft.Core.Server;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Logic.Blocks;

public class TorchBlock : BlockProvider
{
    public enum TorchDirection
    {
        East = 0x01,
        West = 0x02,
        South = 0x03,
        North = 0x04,
        Ground = 0x05
    }

    public static readonly byte BlockID = 0x32;
        
    public override byte ID { get { return 0x32; } }
        
    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 13; } }

    public override bool Opaque { get { return false; } }

    public override bool RenderOpaque { get { return true; } }
        
    public override string GetDisplayName(short metadata)
    {
        return "Torch";
    }

    public override SoundEffectClass SoundEffect
    {
        get
        {
            return SoundEffectClass.Wood;
        }
    }

    public override BoundingBox? BoundingBox { get { return null; } }

    public override BoundingBox? InteractiveBoundingBox
    {
        get
        {
            return new BoundingBox(new Vector3(4 / 16.0, 0, 4 / 16.0), new Vector3(12 / 16.0, 7.0 / 16.0, 12 / 16.0));
        }
    }

    public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IDimension dimension, IRemoteClient user)
    {
        TorchDirection[] preferredDirections =
        {
            TorchDirection.West, TorchDirection.East,
            TorchDirection.North, TorchDirection.South,
            TorchDirection.Ground
        };
        TorchDirection direction;
        switch (face)
        {
            case BlockFace.PositiveZ:
                direction = TorchDirection.South;
                break;
            case BlockFace.NegativeZ:
                direction = TorchDirection.North;
                break;
            case BlockFace.PositiveX:
                direction = TorchDirection.East;
                break;
            case BlockFace.NegativeX:
                direction = TorchDirection.West;
                break;
            default:
                direction = TorchDirection.Ground;
                break;
        }
        int i = 0;
        descriptor.Metadata = (byte)direction;
        while (!IsSupported(dimension, descriptor) && i < preferredDirections.Length)
        {
            direction = preferredDirections[i++];
            descriptor.Metadata = (byte)direction;
        }
        dimension.SetBlockData(descriptor.Coordinates, descriptor);
    }

    public override void ItemUsedOnBlock(GlobalVoxelCoordinates coordinates, ItemStack item, BlockFace face, IDimension dimension, IRemoteClient user)
    {
        ServerOnly.Assert();

        coordinates += MathHelper.BlockFaceToCoordinates(face);
        var old = dimension.GetBlockData(coordinates);
        byte[] overwritable =
        {
            AirBlock.BlockID,
            WaterBlock.BlockID,
            StationaryWaterBlock.BlockID,
            LavaBlock.BlockID,
            StationaryLavaBlock.BlockID
        };
        if (overwritable.Any(b => b == old.ID))
        {
            var data = dimension.GetBlockData(coordinates);
            data.ID = ID;
            data.Metadata = (byte)item.Metadata;

            BlockPlaced(data, face, dimension, user);

            if (!IsSupported(dimension, dimension.GetBlockData(coordinates)))
                dimension.SetBlockData(coordinates, old);
            else
            {
                item.Count--;
                user.Hotbar[user.SelectedSlot].Item = item;
            }
        }
    }

    public override Vector3i GetSupportDirection(BlockDescriptor descriptor)
    {
        switch ((TorchDirection)descriptor.Metadata)
        {
            case TorchDirection.Ground:
                return Vector3i.Down;
            case TorchDirection.East:
                return Vector3i.West;
            case TorchDirection.West:
                return Vector3i.East;
            case TorchDirection.North:
                return Vector3i.South;
            case TorchDirection.South:
                return Vector3i.North;
        }
        return Vector3i.Zero;
    }

    public override Tuple<int, int> GetTextureMap(byte metadata)
    {
        return new Tuple<int, int>(0, 5);
    }
}
