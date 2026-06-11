
using HarmonyLib;
using Shared.TrackData;
using Shared.TrackSelection;
using UnityEngine;

[HarmonyPatch(typeof(BaseTrackSelectionOptionGroup), "InitializeTrackOption")]
static class ToggleProgressBar
{
    static GameObject ProgressBar;

    static void Postfix(BaseTrackSelectionOptionGroup __instance, int optionIndex, ITrackMetadata newTrackMetadata)
    {
        if (newTrackMetadata is FolderTrackMetadata)
        {
            // Folder obj path is Canvas/ScreenContainer/BounceContainer/Content/InfiniteTrackList/ScrollMask/InfiniteScrollTrackArea/TrackOption_Folder(Clone)/
            var container = __instance._options[optionIndex].transform.Find("BounceContainer");
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
        ProgressBar = Object.Instantiate(trackOption.Find("LetterGradeDashed").gameObject);
        ProgressBar.name = "ProgressBar";
        ProgressBar.transform.localPosition = new Vector3(-500, -30, 0);
    }

    static void AttachProgressBar(Transform trackOption)
    {
        var bar = Object.Instantiate(ProgressBar);
        bar.transform.SetParent(trackOption, worldPositionStays: false);
        bar.SetActive(true);
    }
}