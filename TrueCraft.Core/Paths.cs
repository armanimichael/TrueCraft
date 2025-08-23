﻿using System;
using System.IO;

namespace TrueCraft.Core;

public static class Paths
{
    public static string Base
    {
        get
        {
            var xdg_config_home = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            string? config = null;

            if (xdg_config_home is not null)
            {
                config = Path.Combine(xdg_config_home, "truecraft");

                if (Directory.Exists(config))
                {
                    return config;
                }
            }

            var appdata = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "truecraft"
            );

            if (Directory.Exists(appdata))
            {
                return appdata;
            }

            var userprofile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".truecraft"
            );

            if (Directory.Exists(userprofile))
            {
                return userprofile;
            }

            // At this point, there's no existing data to choose from, so use the best option
            if (config is not null)
            {
                Directory.CreateDirectory(config);

                return config;
            }

            Directory.CreateDirectory(appdata);

            return appdata;
        }
    }

    public static string Worlds => Path.Combine(Base, "worlds");

    public static string Settings => Path.Combine(Base, "settings.json");

    public static string Screenshots => Path.Combine(Base, "screenshots");

    public static string TexturePacks => Path.Combine(Base, "texturepacks");
}