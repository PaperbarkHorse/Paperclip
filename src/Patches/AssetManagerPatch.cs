using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Paperclip.Core;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(AssetManager))]
class AssetManagerPatch
{

    [HarmonyPatch("LoadAssetPackage")]
    [HarmonyPrefix]
    private static void LoadAssetPackagePrePatch(ulong assetGUID)
    {
        PaperclipCore.CurrentAssetLoading.Add(assetGUID);
        Paperclip.Logger.LogDebug($"ENTERING MOD {assetGUID}");
    }

    [HarmonyPatch("LoadAssetPackage")]
    [HarmonyPostfix]
    private static void LoadAssetPackagePostPatch(ulong assetGUID)
    {
        ulong topAssetGUID = PaperclipCore.CurrentAssetLoading.Last();
        PaperclipCore.CurrentAssetLoading.RemoveAt(PaperclipCore.CurrentAssetLoading.Count - 1);

        Paperclip.Logger.LogDebug($"LEAVING MOD {topAssetGUID}");
    }

    [HarmonyPatch("RegisterAsset")]
    [HarmonyPostfix]
    private static void RegisterAssetPatch(ref AssetData __result)
    {
        if (__result == null) return;

        if (PaperclipCore.CurrentAssetLoading.Count == 0)
        {
            Paperclip.Logger.LogWarning($"Asset {__result.FilePath} ({__result.GUID}) is not part of a mod that Paperclip knows about");
            return;
        }

        Paperclip.Logger.LogDebug($"Asset {__result.GUID} associated with mod {PaperclipCore.CurrentAssetLoading[0]}");
        PaperclipCore.AssetsBelongingToMods[__result.GUID] = PaperclipCore.CurrentAssetLoading[0];
    }

    [HarmonyPatch("RegisterAssetFromMetaData")]
    [HarmonyPostfix]
    private static void RegisterAssetFromMetaDataPatch(ref AssetData __result)
    {
        if (__result == null) return;

        if (PaperclipCore.CurrentAssetLoading.Count == 0)
        {
            Paperclip.Logger.LogWarning($"Asset (from metadata) {__result.FilePath} ({__result.GUID}) is not part of a mod that Paperclip knows about");
            return;
        }

        Paperclip.Logger.LogDebug($"Asset (from metadata) {__result.GUID} associated with mod {PaperclipCore.CurrentAssetLoading[0]}");
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
        Paperclip.Logger.LogDebug($"GetOrderedTypeSettings for {type.Name}");
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
                if (assetSetting.ParentPackageAssetGUID == ModManager.MainModGUID)
                {
                    list.Insert(0, assetSetting);
                }
                else
                {
                    list.Add(assetSetting);
                }
            }
        }

        List<AssetSetting> loadFirst = new List<AssetSetting>();
        List<AssetSetting> loadNormalMods = new List<AssetSetting>();
        List<AssetSetting> loadPaperclipMods = new List<AssetSetting>();
        List<AssetSetting> loadLast = new List<AssetSetting>();

        foreach (AssetSetting setting in list)
        {
            ulong modGUID;

            Paperclip.Logger.LogDebug($"{setting.GUID} is from mod {PaperclipCore.GetModGUIDForAsset(setting.GUID)}");
            if (PaperclipCore.GetModGUIDForAsset(setting.GUID).IsSome(out modGUID))
            {
                ModMetadata metadata = PaperclipCore.GetModMetadata(modGUID);

                if (modGUID == ModManager.MainModGUID)
                {
                    Paperclip.Logger.LogDebug($"FIRST - {setting.FilePath}");
                    loadFirst.Add(setting);
                }
                else
                {
                    if (metadata.IsPaperclipMod())
                    {
                        Paperclip.Logger.LogDebug($"PAPERCLIP - {setting.FilePath}");
                        loadPaperclipMods.Add(setting);
                    }
                    else
                    {
                        Paperclip.Logger.LogDebug($"NORMAL - {setting.FilePath}");
                        loadNormalMods.Add(setting);
                    }
                }
            }
            else
            {
                Paperclip.Logger.LogDebug($"LAST - {setting.FilePath}");
                loadLast.Add(setting);
            }
        }

        List<AssetSetting> loadOrder = loadFirst
                .Concat(loadNormalMods)
                .Concat(loadPaperclipMods)
                .Concat(loadLast)
                .ToList();

        Paperclip.Logger.LogDebug($"GetOrderedTypeSettings result ->\n{string.Join("\n", loadOrder.Select(v => v.FilePath))}");

        __result = loadOrder;
        return false;
    }

}