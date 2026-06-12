
using System;
using System.Linq;
using HarmonyLib;
using Shared.Pins;
using Shared.PlayerData;
using Shared.StorageData;
using Shared.TrackData;
using Shared.TrackSelection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(BaseTrackSelectionOptionGroup), "InitializeTrackOption")]
static class ToggleProgressBar
{
    static GameObject ProgressBar;

    static void Postfix(BaseTrackSelectionOptionGroup __instance, int optionIndex, ITrackMetadata newTrackMetadata, string folderName)
    {
        if (newTrackMetadata is FolderTrackMetadata)
        {
            var container = __instance._options[optionIndex].transform.Find("BounceContainer/Background");
            var existingBar = container.Find("ProgressBar(Clone)");

            GameObject barInstance = null;
            if (existingBar)
            {
                // Debug
                UnityEngine.Object.Destroy(existingBar.gameObject);

                // existingBar.gameObject.SetActive(true);
            }
            // else
            // {
            if (!ProgressBar)
                CreateProgressBar(container.parent.Find("LetterGradeDashed"));
            barInstance = UnityEngine.Object.Instantiate(ProgressBar);
            barInstance.transform.SetParent(container, worldPositionStays: false);
            barInstance.SetActive(true);

            // }

            int totalTracksInFolder = 0;
            double FCedTracks = 0;

            // Loop through all tracks bc idk how to get idx of newly created folder..
            var trackIndex =
            Enumerable.Range(0, __instance._trackMetaData.Count)
            .First(i => __instance._trackMetaData[i].TrackName == folderName);

            // Go through tracks under folder and count FCs
            while (++trackIndex < __instance._trackMetaData.Count &&
            __instance.GetFolderIndexForTrack(trackIndex) != -1 &&
            __instance._trackMetaData[__instance.GetFolderIndexForTrack(trackIndex)].TrackName == folderName)
            {
                var track = __instance._trackMetaData[trackIndex];

                totalTracksInFolder++;
                if (PlayerDataUtil.GetIsFullComboForDifficulty(track.LevelId, __instance._selectedDifficulty, __instance._options[0].GetStatMode()))
                    FCedTracks++;
            }

            UpdateBarPercentage(barInstance, Math.Round(FCedTracks / totalTracksInFolder, 2));
        }
    }

    const int MaxBarWidth = 300;
    const int MaxBarHeight = 15;
    const int InnerBarMargin = 5;
    static readonly Color OuterBarColor = new Color(0.553f, 0.533f, 0.592f);
    static readonly Color InnerBarColor = Color.black;
    static readonly Vector3 NumberPosition = new Vector3(-200, -40, 0);

    static void CreateProgressBar(Transform textTemplate)
    {
        // TODO: make color change on select (likely something to do with TrackSelectionOptionColorAnimator)
        ProgressBar = new GameObject("ProgressBar");
        ProgressBar.transform.localPosition = new Vector3(200, 0, 0);
        ProgressBar.SetActive(false);

        var outerBar = new GameObject("outerBar");
        outerBar.transform.SetParent(ProgressBar.transform);
        outerBar.transform.localPosition = new Vector3(0, -40, 0);
        outerBar.AddComponent<Image>().color = OuterBarColor;
        outerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(MaxBarWidth, MaxBarHeight);

        var innerBar = UnityEngine.Object.Instantiate(outerBar);
        innerBar.name = "innerBar";
        outerBar.GetComponent<Image>().color = InnerBarColor;
        innerBar.transform.SetParent(outerBar.transform);

        var number = UnityEngine.Object.Instantiate(textTemplate).gameObject;
        number.name = "number";
        number.transform.SetParent(ProgressBar.transform);
        number.transform.localPosition = NumberPosition;
        number.SetActive(true);
    }

    static void UpdateBarPercentage(GameObject bar, double percent)
    {
        var innerBar = bar.transform.Find("outerBar/innerBar");
        innerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(MaxBarWidth * (float)percent, MaxBarHeight - InnerBarMargin);
        var number = bar.transform.Find("number");
        number.GetComponent<TextMeshProUGUI>().text = ((int)(percent * 100)).ToString() + "%";
    }
}