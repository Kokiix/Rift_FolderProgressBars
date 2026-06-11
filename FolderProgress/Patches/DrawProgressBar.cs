
using System;
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

            Debug.LogError("folder " + folderName);

            int totalTracksInFolder = 0;
            double FCedTracks = 0;
            var currFolderIdx = __instance.GetFolderIndexForTrack(++optionIndex);
            while (currFolderIdx != -1)
            {
                totalTracksInFolder++;
                if (__instance._trackMetaData[optionIndex].)
                    Debug.LogError(.TrackName);
                currFolderIdx = __instance.GetFolderIndexForTrack(++optionIndex);
            }
            // var test = __instance._trackMetaData[idx] as FolderTrackMetadata;
            // Debug.LogError(test.TrackName);
            // foreach (Transform t in container.parent.parent.parent)
            // {
            //     if (t.name == "TrackOption_Normal(Clone)")
            //     {
            //         var option = t.gameObject.GetComponent<BaseTrackSelectionOption>();
            //         Debug.LogError(option._trackFolderTabText.text);
            //         if (option._trackFolderTabText.text == folderName)
            //         {
            //             Debug.LogError("song " + option.LevelId + " found under " + folderName);
            //         }
            //         // else
            //         // {
            //         //     Debug.LogError("song " + option.LevelId + " not found under folder");
            //         // }
            //     }
            // }

            // int wholePercentage = (int)Math.Round(FCedTracks / totalTracksInFolder * 100);
            // UpdateBarPercentage(barInstance, wholePercentage);
        }
    }

    const int MaxBarWidth = 300;
    const int MaxBarHeight = 15;
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
        innerBar.name = "innerbar";
        outerBar.GetComponent<Image>().color = InnerBarColor;
        innerBar.transform.SetParent(outerBar.transform);

        var number = UnityEngine.Object.Instantiate(textTemplate).gameObject;
        number.name = "number";
        number.transform.SetParent(ProgressBar.transform);
        number.transform.localPosition = NumberPosition;
        number.SetActive(true);
    }

    static void UpdateBarPercentage(GameObject bar, int percent)
    {
        // var innerBar = bar.transform.Find("outerBar/innerBar");
        // innerBar.GetComponent<RectTransform>().sizeDelta
        // var number = bar.transform.Find("number");
    }
}