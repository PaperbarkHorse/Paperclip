using System.IO;
using HarmonyLib;
using Paperclip.Core;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(ModManager))]
class ModManagerPatch
{

    [HarmonyPatch("LoadAllMods")]
    [HarmonyPrefix]
    private static void PreLoadAllModsPatch(ModManager __instance)
    {
        PaperclipModManager.ClearAllModMetadata();
    }

    [HarmonyPatch("LoadAllMods")]
    [HarmonyPostfix]
    private static void PostLoadAllModsPatch(ModManager __instance)
    {
        LoadBundledAssetMods(__instance);
    }

    private static void LoadBundledAssetMods(ModManager modManager)
    {
        Paperclip.Logger.LogInfo("Loading asset mods bundled with script mods:");

        string installPath = Directory.GetParent(modManager.MainModPath).FullName;
        string pluginsPath = Path.Combine(installPath, "BepInEx/plugins");

        foreach (string pluginPath in Directory.GetDirectories(pluginsPath, "*", SearchOption.TopDirectoryOnly))
        {
            string modsPath = Path.Combine(pluginPath, "AssetMods");

            if (!Directory.Exists(modsPath)) continue;

            foreach (string modPath in Directory.GetDirectories(modsPath, "*", SearchOption.TopDirectoryOnly))
            {
                if (!modPath.CustomEndsWith(".mod")) continue;

                AssetMod assetMod = modManager.LoadExistingMod(modPath);
                modManager.SetIsModEnabled(assetMod.GUID, true);

                PaperclipModMetadata metadata = PaperclipModManager.GetOrCreateModMetadata(assetMod.GUID);
                metadata.BundledByScriptMod = true;

                Paperclip.Logger.LogInfo($" - Loaded {assetMod.ModName} (GUID {assetMod.GUID})");
            }

        }

        Paperclip.Logger.LogInfo("Bundled asset mod loading finished");
    }

}