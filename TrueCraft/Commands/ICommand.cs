using TrueCraft.Core.Networking;

namespace TrueCraft.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Aliases { get; }
        void Handle(IRemoteClient client, string alias, string[] arguments);
        void Help(IRemoteClient client, string alias, string[] arguments);
    }
}
