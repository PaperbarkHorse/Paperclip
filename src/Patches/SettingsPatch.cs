
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Setting;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(Settings))]
class SettingsPatch
{

    [HarmonyPatch("CompileSettingObjects")]
    [HarmonyPrefix]
    private static bool CompileSettingObjectsPatch(Settings __instance, ref Type[] ____settingTypes)
    {
        // Modified decompiled code, overrides base game method

        if (!__instance.IsSettingsCompilationDirty)
        {
            return false;
        }
        __instance.IsSettingsCompilationDirty = false;
        if (____settingTypes == null)
        {
            List<Type> settingTypesToAdd = new List<Type>();

            foreach (Assembly assembly in PaperclipCore.AutoloadAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(SettingBase)))
                    {
                        settingTypesToAdd.Add(type);
                    }
                }
            }

            // List<Type> settingTypesToAdd = (from TheType in Assembly.GetAssembly(typeof(Paperclip)).GetTypes()
            //                                 where TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(typeof(SettingBase))
            //                                 select TheType).ToList();

            ____settingTypes = settingTypesToAdd.ToArray();
            // Paperclip.Logger.LogInfo("AAAAAAAAA");
            // Paperclip.Logger.LogInfo(string.Join("\n", ____settingTypes.Select(v => v.FullName)));
        }
        Type[] settingTypes = ____settingTypes;
        foreach (Type type in settingTypes)
        {
            if (__instance.ListOfDirtySettingTypesToCompile.Count <= 0 || __instance.ListOfDirtySettingTypesToCompile.Contains(type))
            {
                if (Settings.Instance.SettingObjects.ContainsKey(type))
                {
                    ((SettingBase)__instance.SettingObjects[type]).OnDestroy();
                }
                List<AssetSetting> orderedTypeSettings = AssetManager.Instance.GetOrderedTypeSettings(type);
                if (orderedTypeSettings.Count == 0)
                {
                    __instance.SettingObjects[type] = Activator.CreateInstance(type);
                    continue;
                }
                SettingBase settingBase = SettingCompilationUtils.Compile(orderedTypeSettings.ToArray()) as SettingBase;
                settingBase.OnCompiled();
                __instance.SettingObjects[type] = settingBase;
            }
        }
        settingTypes = ____settingTypes;
        foreach (Type type2 in settingTypes)
        {
            if (__instance.ListOfDirtySettingTypesToCompile.Count <= 0 || __instance.ListOfDirtySettingTypesToCompile.Contains(type2))
            {
                ((SettingBase)__instance.SettingObjects[type2]).Start();
            }
        }
        __instance.ListOfDirtySettingTypesToCompile.Clear();
        if (__instance.IsTranslationDirty)
        {
            TranslatedText[] array = UnityEngine.Object.FindObjectsOfType<TranslatedText>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Refresh();
            }
            UIEscapeMenu.IsLayoutDirty = true;
            __instance.IsTranslationDirty = false;
        }

        return false;
    }

}