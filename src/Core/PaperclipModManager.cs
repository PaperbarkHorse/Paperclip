using System;
using System.Collections.Generic;
using System.Reflection;

namespace Paperclip.Core;

public class PaperclipModManager
{

    public static Dictionary<ulong, PaperclipModMetadata> Mods = new Dictionary<ulong, PaperclipModMetadata>();
    public static bool ModRefreshDirty { get; set; } = false;

    public static void ClearAllModMetadata()
    {
        Mods.Clear();
    }

    public static PaperclipModMetadata GetOrCreateModMetadata(ulong modGUID)
    {
        PaperclipModMetadata Metadata;

        if (Mods.TryGetValue(modGUID, out Metadata))
        {
            return Metadata;
        }
        else
        {
            Metadata = new PaperclipModMetadata();
            Metadata.ModGUID = modGUID;
            Mods.Add(modGUID, Metadata);
            return Metadata;
        }
    }

    public static bool HasModMetadata(ulong modGUID)
    {
        return Mods.ContainsKey(modGUID);
    }

    public static bool IsModBundled(ulong modGUID)
    {
        return HasModMetadata(modGUID) && Mods[modGUID].BundledByScriptMod;
    }

    public static void RequestGameRefreshMods()
    {
        Type modManagerType = typeof(ModManager);

        modManagerType.GetField("_isEnabledModDirty", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ModManager.Instance, true);

        modManagerType.GetField("_showLoadingScreen", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ModManager.Instance, true);

        ModRefreshDirty = false;
    }

}