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
        if (!PaperclipModManager.HasModMetadata(__instance.GUID)) return;
        Paperclip.Logger.LogInfo($"WriteMetaFileData - Start {__instance.GUID}");

        PaperclipModMetadata metadata = PaperclipModManager.GetOrCreateModMetadata(__instance.GUID);

        textWriter.WriteLine("PaperclipRequiredDependencies:{0}", PaperclipUtils.SerializeGUIDList(metadata.RequiredDependencyGUIDs));
        textWriter.WriteLine("PaperclipOptionalDependencies:{0}", PaperclipUtils.SerializeGUIDList(metadata.OptionalDependencyGUIDs));

        Paperclip.Logger.LogInfo($"WriteMetaFileData - Complete {__instance.GUID}");
    }

    [HarmonyPatch(nameof(AssetMod.ReadMetaFileLine))]
    [HarmonyPostfix]
    private static void ReadMetaFileLinePatch(AssetMod __instance, string key, string value)
    {
        if (key != null && !key.StartsWith("Paperclip")) return;
        PaperclipModMetadata metadata = PaperclipModManager.GetOrCreateModMetadata(__instance.GUID);

        switch (key)
        {
            case "PaperclipRequiredDependencies":
                metadata.RequiredDependencyGUIDs = PaperclipUtils.DeserializeGUIDList(value);
                Paperclip.Logger.LogInfo($"ReadMetaFileLine - Required dependencies loaded {__instance.GUID} -> {value}");
                break;
            case "PaperclipOptionalDependencies":
                metadata.OptionalDependencyGUIDs = PaperclipUtils.DeserializeGUIDList(value);
                Paperclip.Logger.LogInfo($"ReadMetaFileLine - Optional dependencies loaded {__instance.GUID} -> {value}");
                break;
        }
    }

}