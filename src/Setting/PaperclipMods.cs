using System.Collections.Generic;
using System.Linq;
using Paperclip;

namespace Setting;

[GenerateSettingAPI]
public class PaperclipMods : SettingBase
{
    public PaperclipModSetting[] Mods;

    public override void OnCompiled()
    {
        base.OnCompiled();

        List<PaperclipModSetting> modSettings = new List<PaperclipModSetting>();

        foreach (ulong modGUID in ModManager.Instance.Mods)
        {

            AssetMod assetMod = ModManager.Instance.GetAssetModByGUID(modGUID);
            if (assetMod == null)
            {
                PaperclipPlugin.Logger.LogError($"Skipping reading settings for mod {modGUID} because it could not be found");
                continue;
            }

            if (!assetMod.Enabled) continue;

            ModMetadata metadata = PaperclipCore.GetModMetadata(modGUID);

            if (!metadata.IsPaperclipMod()) continue;

            modSettings.Add(new PaperclipModSetting()
            {
                Mod = modGUID,
                Dependencies = metadata.DependencyGUIDs
                        .Select((v) => new PaperclipModDependencySetting() { Dependency = v })
                        .ToArray()
            });
        }

        Mods = modSettings.ToArray();
    }

    public override void UpdateWhenDirty()
    {
        PaperclipPlugin.Logger.LogDebug("UpdateWhenDirty");

        base.UpdateWhenDirty();

        foreach (PaperclipModSetting modSetting in Mods)
        {
            PaperclipPlugin.Logger.LogDebug($"1 - {modSetting.Mod}");

            if (ModManager.Instance.ModBeingEdited != modSetting.Mod) continue;
            PaperclipPlugin.Logger.LogDebug($"2");

            AssetMod assetMod = ModManager.Instance.GetAssetModByGUID(modSetting.Mod);
            if (assetMod == null)
            {
                PaperclipPlugin.Logger.LogError($"Skipping changing settings for mod {modSetting.Mod} because it could not be found");
                continue;
            }

            PaperclipPlugin.Logger.LogDebug($"3 - {assetMod.ModName}");

            ModMetadata metadata = PaperclipCore.GetModMetadata(modSetting.Mod);
            metadata.MarkAsPaperclipMod();

            metadata.DependencyGUIDs = modSetting.Dependencies
                    .Where((v) => v.Dependency != 0)
                    .Select((v) => v.Dependency)
                    .ToList();

            PaperclipPlugin.Logger.LogDebug($"4 - {metadata.DependencyGUIDs}");

            AssetManager.Instance.WriteMetaFile(assetMod);
            PaperclipPlugin.Logger.LogDebug($"5");
        }
    }
}

[ClassGUID("Mod", "", "")]
[ArrayItemNameBasedOnAssetName("Mod")]
public class PaperclipModSetting
{
    [AssetInspector(AssetType.Mod)]
    public ulong Mod;

    public PaperclipModDependencySetting[] Dependencies;
}

[ClassGUID("Dependency", "", "")]
[ArrayItemNameBasedOnAssetName("Dependency")]
public class PaperclipModDependencySetting
{
    [AssetInspector(AssetType.Mod)]
    public ulong Dependency;
}