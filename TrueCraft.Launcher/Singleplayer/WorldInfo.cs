using fNbt;

namespace TrueCraft.Launcher.Singleplayer;

public class WorldInfo
{
    public WorldInfo(string directory, NbtFile manifest)
    {
        Directory = directory;
        Seed = manifest.RootTag["Seed"].IntValue;
        Name = manifest.RootTag["Name"].StringValue;
    }

    /// <summary>
    /// Gets the name of the Directory in which this World is saved.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    /// Gets the Seed used to generate the World.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Gets the name of the World
    /// </summary>
    public string Name { get; }
}