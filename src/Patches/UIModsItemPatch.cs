using HarmonyLib;
using Paperclip.Core;
using Steamworks.Data;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(UIModsItem))]
class UIModsItemPatch
{

    [HarmonyPatch(nameof(UIModsItem.Init))]
    [HarmonyPostfix]
    private static void InitPatch(UIModsItem __instance, ulong modGUID)
    {
        if (PaperclipCore.IsModBundled(modGUID))
        {

            __instance.SubscribedOwnedModIcon.SetActive(false);
            __instance.SubscribedModIcon.SetActive(false);
            // __instance.ButtonRenameMod.SetActive(false);
            // __instance.ButtonSetPreviewImage.SetActive(false);
            __instance.ToggleIsModBeingEdited.gameObject.SetActive(false);
            __instance.UploadUpdateButtonObject.SetActive(false);
            __instance.ButtonWorkshopVisibility.SetActive(false);

            __instance.transform.Find("ToggleIsModEnabled")?.gameObject?.SetActive(false);
            __instance.transform.Find("TopRightButtons/ButtonDelete")?.gameObject?.SetActive(false);
            __instance.transform.Find("TopRightButtons/ButtonViewWorkshop")?.gameObject?.SetActive(false);
        }
    }

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void UpdatePatch(UIModsItem __instance)
    {
        __instance.PanelModDisabledWhenEditingOtherMod.SetActive(false);
    }

    [HarmonyPatch("RefreshModName")]
    [HarmonyPostfix]
    private static void RefreshModNamePatch(UIModsItem __instance, ulong ____modGUID)
    {
        var modGUID = ____modGUID;
        if (PaperclipCore.IsModBundled(modGUID))
        {
            // BUG: Game doesn't read the translation key from the mod properly, this seems to be
            //      a base game bug that'll need patching :'3
            __instance.LabelModType.Key = "Paperclip_UIMods_ModTypeScriptBundled";
            __instance.LabelModType.Refresh();
        }
    }

}