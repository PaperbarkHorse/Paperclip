using System.IO;
using HarmonyLib;
using Paperclip;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(AssetMod))]
class AssetModPatch
{

    [HarmonyPatch(nameof(AssetMod.WriteMetaFileData))]
    [HarmonyPostfix]
    private static void WriteMetaFileDataPatch(AssetMod __instance, TextWriter textWriter)
    {
        if (!PaperclipCore.HasModMetadata(__instance.GUID)) return;

        ModMetadata metadata = PaperclipCore.GetModMetadata(__instance.GUID);

        if (metadata.PaperclipVersion > 0)
        {
            textWriter.WriteLine("PaperclipVersion:{0}", metadata.PaperclipVersion);
            textWriter.WriteLine("PaperclipDependencies:{0}", PaperclipUtils.SerializeGUIDList(metadata.DependencyGUIDs));
        }

        PaperclipPlugin.Logger.LogDebug($"WriteMetaFileData - Complete {__instance.GUID}");
    }

    [HarmonyPatch(nameof(AssetMod.ReadMetaFileLine))]
    [HarmonyPostfix]
    private static void ReadMetaFileLinePatch(AssetMod __instance, string key, string value)
    {
        if (key != null && !key.StartsWith("Paperclip")) return;
        ModMetadata metadata = PaperclipCore.GetModMetadata(__instance.GUID);

        switch (key)
        {
            case "PaperclipVersion":
                metadata.PaperclipVersion = int.Parse(value);
                break;
            case "PaperclipDependencies":
                metadata.DependencyGUIDs = PaperclipUtils.DeserializeGUIDList(value);
                break;
        }
    }

}