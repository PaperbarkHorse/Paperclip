using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Paperclip.Patches;

namespace Paperclip;

[BepInPlugin(GUID, NAME, VERSION)]
public class PaperclipPlugin : BaseUnityPlugin
{
    public const string GUID = "horse.paperbark.paperclip";
    public const string NAME = "Paperclip";
    public const string VERSION = "0.1.0";

    internal static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(NAME);
    private static Harmony harmony = new Harmony(GUID);

    internal static new PaperclipConfig Config { get; private set; } = null!;

    private void Awake()
    {
        Config = new PaperclipConfig(base.Config);

        harmony.PatchAll(typeof(AssetManagerPatch));
        harmony.PatchAll(typeof(AssetModPatch));
        harmony.PatchAll(typeof(AssetSettingPatch));
        harmony.PatchAll(typeof(ModManagerPatch));
        harmony.PatchAll(typeof(SettingsPatch));
        harmony.PatchAll(typeof(UIMainMenuPatch));
        harmony.PatchAll(typeof(UIModsItemPatch));
        harmony.PatchAll(typeof(UIModsPatch));

        Logger.LogInfo($"Paperclip {VERSION} is loaded. Happy modding! /)");
    }
}
