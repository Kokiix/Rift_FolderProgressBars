
using HarmonyLib;
using Shared.TrackData;
using Shared.TrackSelection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(BaseTrackSelectionOptionGroup), "InitializeTrackOption")]
static class ToggleProgressBar
{
    static GameObject ProgressBar;

    static void Postfix(BaseTrackSelectionOptionGroup __instance, int optionIndex, ITrackMetadata newTrackMetadata)
    {
        if (newTrackMetadata is FolderTrackMetadata)
        {
            var container = __instance._options[optionIndex].transform.Find("BounceContainer/Background");
            var existingBar = container.Find("ProgressBar(Clone)");
            if (existingBar)
            {
                // Debug
                Object.Destroy(existingBar.gameObject);

                // existingBar.gameObject.SetActive(true);
            }
            // else
            // {
            if (!ProgressBar)
                CreateProgressBar(container);
            AttachProgressBar(container);
            // }
        }
    }

    static void CreateProgressBar(Transform trackOption)
    {
        // TODO: make color change on select (likely something to do with TrackSelectionOptionColorAnimator)
        ProgressBar = new GameObject("ProgressBar");
        ProgressBar.transform.localPosition = new Vector3(200, 0, 0);
        ProgressBar.SetActive(false);

        var outerBar = new GameObject("outerBar");
        outerBar.transform.SetParent(ProgressBar.transform);
        outerBar.transform.localPosition = new Vector3(0, -40, 0);
        outerBar.AddComponent<Image>().color = new Color(0.553f, 0.533f, 0.592f);
        outerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 15);

        var innerBar = Object.Instantiate(outerBar);
        innerBar.transform.SetParent(outerBar.transform);
        innerBar.transform.localPosition = new Vector3(-72.5f, 0, 0);
        innerBar.GetComponent<Image>().color = Color.black;
        innerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 10);

        var number = Object.Instantiate(trackOption.parent.Find("LetterGradeDashed")).gameObject;
        number.transform.SetParent(ProgressBar.transform);
        number.transform.localPosition = new Vector3(-200, -40, 0);
        number.GetComponent<TextMeshProUGUI>().text = "50%";
        number.SetActive(true);
    }

    static void AttachProgressBar(Transform trackOption)
    {
        var bar = Object.Instantiate(ProgressBar);
        bar.transform.SetParent(trackOption, worldPositionStays: false);
        bar.SetActive(true);
    }
}