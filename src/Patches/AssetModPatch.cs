using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Paperclip.Core;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(AssetMod))]
class AssetModPatch
{

    [HarmonyPatch(nameof(AssetMod.WriteMetaFileData))]
    [HarmonyPostfix]
    private static void WriteMetaFileDataPatch(AssetMod __instance, TextWriter textWriter)
    {
        if (!Paperclip.HasModMetadata(__instance.GUID)) return;

        ModMetadata metadata = Paperclip.GetModMetadata(__instance.GUID);

        textWriter.WriteLine("PaperclipRequiredDependencies:{0}", PaperclipUtils.SerializeGUIDList(metadata.RequiredDependencyGUIDs));
        textWriter.WriteLine("PaperclipOptionalDependencies:{0}", PaperclipUtils.SerializeGUIDList(metadata.OptionalDependencyGUIDs));

        PaperclipPlugin.Logger.LogDebug($"WriteMetaFileData - Complete {__instance.GUID}");
    }

    [HarmonyPatch(nameof(AssetMod.ReadMetaFileLine))]
    [HarmonyPostfix]
    private static void ReadMetaFileLinePatch(AssetMod __instance, string key, string value)
    {
        if (key != null && !key.StartsWith("Paperclip")) return;
        ModMetadata metadata = Paperclip.GetModMetadata(__instance.GUID);

        switch (key)
        {
            case "PaperclipRequiredDependencies":
                metadata.RequiredDependencyGUIDs = PaperclipUtils.DeserializeGUIDList(value);
                break;
            case "PaperclipOptionalDependencies":
                metadata.OptionalDependencyGUIDs = PaperclipUtils.DeserializeGUIDList(value);
                break;
        }
    }

}