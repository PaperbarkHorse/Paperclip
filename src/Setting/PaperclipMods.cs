namespace Setting;

[GenerateSettingAPI]
public class PaperclipMods : SettingBase
{
    public PaperclipModSetting[] Mods;
}

[ClassGUID("GUID", "DisplayName", "")]
public class PaperclipModSetting
{
    public ulong GUID;
    public string DisplayName;

    [AssetInspector(AssetType.Mod)]
    public ulong Mod;

    public PaperclipModDependencySetting[] Dependencies;
}

[ClassGUID("GUID", "DisplayName", "")]
public class PaperclipModDependencySetting
{
    public ulong GUID;
    public string DisplayName;

    [AssetInspector(AssetType.Mod)]
    public ulong Dependency;

    public bool Required;
}