using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Paperclip.Core;

public class PaperclipCore
{

    public static Dictionary<ulong, ModMetadata> Mods = new Dictionary<ulong, ModMetadata>();
    public static Dictionary<ulong, ulong> AssetsBelongingToMods = new Dictionary<ulong, ulong>();

    public static List<ulong> CurrentAssetLoading = new List<ulong>();

    public static ModMetadata GetModMetadata(ulong modGUID)
    {
        ModMetadata Metadata;

        if (Mods.TryGetValue(modGUID, out Metadata))
        {
            return Metadata;
        }
        else
        {
            Metadata = new ModMetadata();
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

    public static void RefreshMods()
    {
        Type modManagerType = typeof(ModManager);

        modManagerType.GetField("_isEnabledModDirty", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ModManager.Instance, true);

        modManagerType.GetField("_showLoadingScreen", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(ModManager.Instance, true);
    }

    public static Option<ulong> GetModGUIDForAsset(ulong assetGUID)
    {
        if (AssetsBelongingToMods.ContainsKey(assetGUID))
        {
            return Option<ulong>.Some(AssetsBelongingToMods[assetGUID]);
        }
        else
        {
            return Option<ulong>.None;
        }
    }

}