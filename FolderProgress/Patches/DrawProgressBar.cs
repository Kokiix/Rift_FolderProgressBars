
using HarmonyLib;
using Shared.TrackData;
using Shared.TrackSelection;
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
        ProgressBar = new GameObject("ProgressBar");
        ProgressBar.transform.localPosition = new Vector3(-250, -30, 0);
        ProgressBar.SetActive(false);

        var img = ProgressBar.AddComponent<Image>();
        img.color = Color.blue;

        var rect = ProgressBar.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 100);
    }

    static void AttachProgressBar(Transform trackOption)
    {
        var bar = Object.Instantiate(ProgressBar);
        bar.transform.SetParent(trackOption, worldPositionStays: false);
        bar.SetActive(true);
    }
}