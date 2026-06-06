using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Paperclip.Patches;
using Setting;

namespace Paperclip;

[BepInPlugin(GUID, NAME, VERSION)]
public class Paperclip : BaseUnityPlugin
{
    public const string GUID = "horse.paperbark.paperclip";
    public const string NAME = "Paperclip";
    public const string VERSION = "0.1.0";

    internal static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(NAME);
    private static Harmony harmony = new Harmony(GUID);

    public static PaperclipGlobal global = new PaperclipGlobal();

    private void Awake()
    {
        harmony.PatchAll(typeof(AssetManagerPatch));
        harmony.PatchAll(typeof(AssetModPatch));
        harmony.PatchAll(typeof(ModManagerPatch));
        harmony.PatchAll(typeof(SettingsPatch));
        harmony.PatchAll(typeof(UIMainMenuPatch));
        harmony.PatchAll(typeof(UIModsItemPatch));
        harmony.PatchAll(typeof(UIModsPatch));

        Logger.LogInfo($"Paperclip {VERSION} is loaded. Happy modding! /)");
    }
}
