﻿using System;
using TrueCraft.Core.Networking;

namespace TrueCraft.Commands
{
    public class HelpCommand : Command
    {
        public override string Name
        {
            get { return "help"; }
        }

        public override string Description
        {
            get { return "Command help menu."; }
        }

        public override void Handle(IRemoteClient client, string alias, string[] arguments)
        {
            if (arguments.Length > 1)
            {
                Help(client, alias, arguments);
                return;
            }

            var identifier = arguments.Length == 1 ? arguments[0] : "1";

            ICommand? found;
            if ((found = CommandManager.Instance.FindByName(identifier)) is not null)
            {
                found.Help(client, identifier, new string[0]);
                return;
            }
            else if ((found = CommandManager.Instance.FindByAlias(identifier)) is not null)
            {
                found.Help(client, identifier, new string[0]);
                return;
            }

            int pageNumber;
            if (int.TryParse(identifier, out pageNumber))
            {
                HelpPage(client, pageNumber);
                return;
            }
            Help(client, alias, arguments);
        }

        public void HelpPage(IRemoteClient client, int page)
        {
            const int perPage = 5;
            int numPages = (int)Math.Floor(((double)CommandManager.Instance.Count / perPage));
            if ((CommandManager.Instance.Count % perPage) > 0)
                numPages++;

            if (page < 1 || page > numPages)
                page = 1;

            int startingIndex = (page - 1) * perPage;
            client.SendMessage("--Help page " + page + " of " + numPages + "--");
            for (int i = 0; i < perPage; i++)
            {
                int index = startingIndex + i;
                if (index > CommandManager.Instance.Count - 1)
                {
                    break;
                }
                ICommand command = CommandManager.Instance[index];
                client.SendMessage("/" + command.Name + " - " + command.Description);
            }
        }

        public override void Help(IRemoteClient client, string alias, string[] arguments)
        {
            client.SendMessage("Correct usage is /" + alias + " <page#/command> [command arguments]");
        }
    }
}