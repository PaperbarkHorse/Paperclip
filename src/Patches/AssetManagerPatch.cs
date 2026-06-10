using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Paperclip;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(AssetManager))]
class AssetManagerPatch
{

    [HarmonyPatch("LoadAssetPackage")]
    [HarmonyPrefix]
    private static void LoadAssetPackagePrePatch(ulong assetGUID)
    {
        PaperclipCore.CurrentAssetLoading.Add(assetGUID);
    }

    [HarmonyPatch("LoadAssetPackage")]
    [HarmonyPostfix]
    private static void LoadAssetPackagePostPatch(ulong assetGUID)
    {
        PaperclipCore.CurrentAssetLoading.RemoveAt(PaperclipCore.CurrentAssetLoading.Count - 1);
    }

    [HarmonyPatch("RegisterAsset")]
    [HarmonyPostfix]
    private static void RegisterAssetPatch(ref AssetData __result)
    {
        if (__result == null) return;

        if (PaperclipCore.CurrentAssetLoading.Count == 0)
        {
            PaperclipPlugin.Logger.LogWarning($"Asset {__result.FilePath} ({__result.GUID}) is not part of a mod that Paperclip knows about");
            return;
        }

        PaperclipCore.AssetsBelongingToMods[__result.GUID] = PaperclipCore.CurrentAssetLoading[0];
    }

    [HarmonyPatch("RegisterAssetFromMetaData")]
    [HarmonyPostfix]
    private static void RegisterAssetFromMetaDataPatch(ref AssetData __result)
    {
        if (__result == null) return;

        if (PaperclipCore.CurrentAssetLoading.Count == 0)
        {
            PaperclipPlugin.Logger.LogWarning($"Asset (from metadata) {__result.FilePath} ({__result.GUID}) is not part of a mod that Paperclip knows about");
            return;
        }

        PaperclipCore.AssetsBelongingToMods[__result.GUID] = PaperclipCore.CurrentAssetLoading[0];
    }

    [HarmonyPatch("GetOrderedTypeSettings")]
    [HarmonyPrefix]
    private static bool GetOrderedTypeSettingsPatch(
        AssetManager __instance,
        ref List<AssetSetting> __result,
        ref Dictionary<ulong, AssetData> ____assets,
        Type type
    )
    {
        List<AssetSetting> list = new List<AssetSetting>();
        foreach (KeyValuePair<ulong, AssetData> asset in ____assets)
        {
            if (!(asset.Value is AssetSetting))
            {
                continue;
            }
            AssetSetting assetSetting = asset.Value as AssetSetting;
            if (assetSetting.IsSettingType && !(assetSetting.SettingType != type))
            {
                list.Add(assetSetting);
            }
        }

        List<AssetSetting> loadFirst = new List<AssetSetting>();
        List<AssetSetting> loadMods = new List<AssetSetting>();
        List<AssetSetting> loadPaperclipMods = new List<AssetSetting>();
        List<AssetSetting> loadLast = new List<AssetSetting>();

        foreach (AssetSetting setting in list)
        {
            ulong modGUID;

            if (PaperclipCore.GetModGUIDForAsset(setting.GUID).IsSome(out modGUID))
            {
                ModMetadata metadata = PaperclipCore.GetModMetadata(modGUID);

                if (modGUID == ModManager.MainModGUID)
                {
                    loadFirst.Add(setting);
                }
                else
                {
                    if (metadata.IsPaperclipMod())
                    {
                        loadPaperclipMods.Add(setting);
                    }
                    else
                    {
                        loadMods.Add(setting);
                    }
                }
            }
            else
            {
                loadLast.Add(setting);
            }
        }

        List<AssetSetting> loadOrder = loadFirst
                .Concat(loadMods)
                .Concat(loadPaperclipMods)
                .Concat(loadLast)
                .ToList();

        PaperclipPlugin.Logger.LogDebug($"GetOrderedTypeSettings {type.Name} result ->\n{string.Join("\n", loadOrder.Select(v => v.FilePath))}");

        __result = loadOrder;
        return false;
    }

}