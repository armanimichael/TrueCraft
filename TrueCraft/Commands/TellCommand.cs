using System;
using System.Linq;
using TrueCraft.Core;
using TrueCraft.Core.Networking;

namespace TrueCraft.Commands;

public class TellCommand : Command
{
    public override string Name => "tell";

    public override string Description => "Whisper someone a message.";

    public override string[] Aliases => new string[0];

    public override void Handle(IRemoteClient client, string alias, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Help(client, alias, arguments);

            return;
        }

        var username = arguments[0];
        var messageBuilder = new System.Text.StringBuilder();

        for (var i = 1; i < arguments.Length; i++)
        {
            messageBuilder.Append(arguments[i] + " ");
        }

        var message = messageBuilder.ToString();

        var receivingPlayer = GetPlayerByName(client, username);

        if (receivingPlayer is null)
        {
            client.SendMessage("No client with the username \"" + username + "\" was found.");

            return;
        }

        if (receivingPlayer == client)
        {
            client.SendMessage(ChatColor.Red + "You can't send a private message to yourself!");

            return;
        }

        receivingPlayer.SendMessage(ChatColor.Gray + "<" + client.Username + " -> You> " + message);
    }

    protected static IRemoteClient? GetPlayerByName(IRemoteClient client, string username)
    {
        var receivingPlayer =
            client.Server.Clients.FirstOrDefault(
                c =>
                    string.Equals(c.Username, username, StringComparison.CurrentCultureIgnoreCase)
            );

        return receivingPlayer;
    }

    public override void Help(IRemoteClient client, string alias, string[] arguments) => client.SendMessage("Correct usage is /" + alias + "<player> <message>");
}