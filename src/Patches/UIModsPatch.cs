using HarmonyLib;
using Paperclip.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(UIMods))]
class UIModsPatch
{

    [HarmonyPatch("RefreshUI")]
    [HarmonyPostfix]
    private static void RefreshUIPatch(UIMods __instance)
    {
        RefreshRefreshModsButton(__instance);
    }

    private static void RefreshRefreshModsButton(UIMods instance)
    {
        GameObject content = instance.transform.Find("Content")?.gameObject;
        GameObject gameObject = content.transform.Find("PaperclipRefreshModsButton")?.gameObject;

        if (!gameObject)
        {
            gameObject = Object.Instantiate(content.transform.Find("ButtonCreateMod")?.gameObject, content.transform);
            gameObject.name = "PaperclipRefreshModsButton";

            ParaButton paraButton = gameObject.GetComponent<ParaButton>();

            paraButton.ButtonClick = new UnityEvent();
            paraButton.ButtonClick.AddListener(OnRefreshModsButtonClick);

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-240, 295);
            gameObject.GetComponentInChildren<TranslatedText>().Key = "Paperclip_UIMods_RefreshMods";
        }
    }

    private static void OnRefreshModsButtonClick()
    {
        PaperclipModManager.RequestGameRefreshMods();
    }

}