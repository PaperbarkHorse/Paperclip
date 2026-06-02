using System.Collections.Generic;

namespace Paperclip.Core;

public class PaperclipModManager
{

    public static Dictionary<ulong, PaperclipModMetadata> Mods = new Dictionary<ulong, PaperclipModMetadata>();

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

}