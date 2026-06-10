using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace Paperclip;

class PaperclipConfig
{
    public readonly ConfigEntry<bool> EnableModManager;

    public PaperclipConfig(ConfigFile config)
    {
        config.SaveOnConfigSet = false;

        EnableModManager = config.Bind(
            "ModManager",
            "Enabled",
            true,
            "Whether Paperclip should replace the base game's mod manager with its own"
        );

        ClearOrphanedEntries(config);
        config.Save();
        config.SaveOnConfigSet = true;
    }

    static void ClearOrphanedEntries(ConfigFile cfg)
    {
        PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
        orphanedEntries.Clear();
    }
}