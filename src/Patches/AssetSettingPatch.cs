using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(AssetSetting))]
class AssetSettingPatch
{

    [HarmonyPatch(typeof(AssetSetting), MethodType.Constructor, [typeof(string)])]
    [HarmonyPostfix]
    private static void ConstructorPatch(AssetSetting __instance, string filePath)
    {
        // Game failed to find setting type, it's probably modded
        if (__instance.SettingType == null)
        {
            if (new FileInfo(__instance.FilePath).Length > 0)
            {
                ManifestItem manifestItem = new ManifestItem(File.ReadLines(__instance.FilePath).First());
                bool IsSettingType = manifestItem.Operation == ManifestOperation.Object;
                if (IsSettingType)
                {
                    __instance.SettingType = Paperclip.GetTypeFromAutoloadAssemblies(manifestItem.FieldName);

                    if (__instance.SettingType == null)
                    {
                        PaperclipPlugin.Logger.LogWarning($"Failed to find type {manifestItem.FieldName} while constructing AssetSetting at location {filePath}");
                    }
                }
            }
        }
    }

    [HarmonyPatch("ReadMetaFileLine")]
    [HarmonyPostfix]
    private static void ReadMetaFileLinePatch(AssetSetting __instance, string key, string value)
    {
        // Check if setting not found from previous step
        if (key == "SettingType" && __instance.SettingType == null)
        {
            string typeName = value.Trim();
            __instance.SettingType = Paperclip.GetTypeFromAutoloadAssemblies(typeName);

            if (__instance.SettingType == null)
            {
                PaperclipPlugin.Logger.LogWarning($"Failed to find type {typeName} while reading meta for AssetSetting at location {__instance.FilePath}");
            }
        }
    }
}