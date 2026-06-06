using HarmonyLib;
using UnityEngine;

namespace Paperclip.Patches;

[HarmonyPatch(typeof(UIMainMenu))]
class UIMainMenuPatch
{

    [HarmonyPatch("OnShow")]
    [HarmonyPostfix]
    private static void OnShowPatch(UIMainMenu __instance)
    {
        RefreshPaperclipLabel(__instance);
    }

    private static void RefreshPaperclipLabel(UIMainMenu instance)
    {
        GameObject content = instance.transform.Find("Content")?.gameObject;
        GameObject gameObject = content.transform.Find("PaperclipVersionLabel")?.gameObject;

        if (!gameObject)
        {
            gameObject = Object.Instantiate(content.transform.Find("LabelGameVersion")?.gameObject, content.transform);
            gameObject.name = "PaperclipVersionLabel";

            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(227, 70);
        }

        TranslatedText translatedText = gameObject.GetComponentInChildren<TranslatedText>();
        translatedText.SetParameters(PaperclipPlugin.VERSION);
        translatedText.Key = "Paperclip_Version_Label";
    }

}