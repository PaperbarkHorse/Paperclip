using System.IO;
using HarmonyLib;
using Paperclip.Core;
using Setting;
using UnityEngine;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(ModManager))]
class ModManagerPatch
{

    [HarmonyPatch("RefreshCurrentlyLoadedMods")]
    [HarmonyPrefix]
    private static bool RefreshCurrentlyLoadedModsPatch(
        ModManager __instance,
        ref bool ____isEnabledModDirty,
        ref bool ____showLoadingScreen,
        ref float ____waitingForLoadingScreen
    )
    {
        if (!____isEnabledModDirty)
        {
            return false;
        }
        if (____showLoadingScreen)
        {
            UI.Get<UIFloatingText>(0).ShowAtCenterOfScreen(Settings.Get<FloatingText>().ProfileGameSaved, TranslationManager.Get("UILoadingScreen_Loading"));
            ____showLoadingScreen = false;
            ____waitingForLoadingScreen = 0.5f;
            return false;
        }
        if (____waitingForLoadingScreen > 0f)
        {
            ____waitingForLoadingScreen -= Time.fixedDeltaTime;
            return false;
        }

        PaperclipPlugin.Logger.LogInfo("Refreshing currently loaded mods");
        ____isEnabledModDirty = false;

        bool flag = false;
        for (int i = 0; i < __instance.Mods.Count; i++)
        {
            ulong num = __instance.Mods[i];
            AssetMod assetMod = (AssetMod)AssetManager.Instance.GetAsset(num);
            bool flag2 = assetMod.Enabled || __instance.IsBaseMod(num);
            if (flag2 && !assetMod.IsLoaded)
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                AssetManager.FullAssetLoadingProfiler?.Start("LoadAssetPackageRecursive", "RefreshCurrentlyLoadedMods", 278, "C:\\Users\\poik0\\Documents\\paralives\\paralives\\Assets\\Scripts\\Mods\\ModManager.cs");
                AssetManager.MetacacheStatus metacacheStatus = AssetManager.Instance.LoadAssetPackage(num);
                PaperclipPlugin.Logger.LogInfo(string.Format("Loaded asset database{0}of mod {1} in {2} seconds.", metacacheStatus switch
                {
                    AssetManager.MetacacheStatus.ForceUseGlobalCache => " (From global metacache) ",
                    AssetManager.MetacacheStatus.RebuildCacheIfOld => " ",
                    AssetManager.MetacacheStatus.NoCache => " (No metacache) ",
                    _ => "",
                }, assetMod.ModName, Time.realtimeSinceStartup - realtimeSinceStartup));
                AssetManager.FullAssetLoadingProfiler?.StopOneLevel();
                AssetManager.FullAssetLoadingProfiler?.PrintAll(includeCallingFunctionNames: true);
                AssetManager.FullAssetLoadingProfiler = null;
                flag = true;
            }
            else if (!flag2 && assetMod.IsLoaded)
            {
                assetMod.Unload();
                MonoBehaviour.print("Unloaded asset database of mod " + assetMod.ModName);
                flag = true;
                if (__instance.ModBeingEdited == num)
                {
                    typeof(ModManager).GetProperty("ModBeingEdited").GetSetMethod(true).Invoke(__instance, [0uL]);
                }
            }
        }
        if (flag)
        {
            Settings.Instance.IsSettingsCompilationDirty = true;
            VoiceOverManager.Instance.RefreshVoiceOverAudioList();
            FootstepManager.Instance.RefreshFootstepAudioList();
        }
        __instance.LoadLanguageModBasedOnCurrentLanguage();

        return false;
    }

    [HarmonyPatch("LoadAllMods")]
    [HarmonyPostfix]
    private static void LoadAllModsPostPatch(ModManager __instance)
    {
        LoadBundledAssetMods(__instance);
    }

    private static void LoadBundledAssetMods(ModManager modManager)
    {
        PaperclipPlugin.Logger.LogInfo("Loading asset mods bundled with script mods:");

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

                ModMetadata metadata = Paperclip.GetModMetadata(assetMod.GUID);
                metadata.BundledByScriptMod = true;

                PaperclipPlugin.Logger.LogInfo($" - Loaded {assetMod.ModName} (GUID {assetMod.GUID})");
            }

        }

        PaperclipPlugin.Logger.LogInfo("Bundled asset mod loading finished");
    }

    [HarmonyPatch("SetIsModEnabled")]
    [HarmonyPrefix]
    private static bool SetIsModEnabledPatch(ModManager __instance, ulong guid, bool isEnabled)
    {
        AssetMod assetMod = (AssetMod)AssetManager.Instance.GetAsset(guid);
        if (!assetMod.IsMainMod && assetMod.Enabled != isEnabled)
        {
            assetMod.Enabled = isEnabled;
            AssetManager.Instance.WriteMetaFile(assetMod);
        }

        return false;
    }

}