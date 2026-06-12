
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
static class ApplyBarToNewOption
{
    static void Postfix(BaseTrackSelectionOptionGroup __instance, int optionIndex, ITrackMetadata newTrackMetadata, string folderName)
    {
        if (newTrackMetadata is FolderTrackMetadata)
        {
            ProgressBar.ApplyBarToFolder(__instance, optionIndex, folderName);
        }
    }
}

[HarmonyPatch(typeof(BaseTrackSelectionOptionGroup), "HandleDifficultyChanged")]
static class ApplyBarToAllOptions
{
    static void Postfix(BaseTrackSelectionOptionGroup __instance)
    {
        for (int i = 0; i < __instance._options.Count; i++)
        {
            var meta = __instance._options[i].Metadata;
            if (meta is FolderTrackMetadata)
                ProgressBar.ApplyBarToFolder(__instance, i, meta.TrackName);
        }
    }
}

static class ProgressBar
{
    static GameObject Template;

    internal static void ApplyBarToFolder(BaseTrackSelectionOptionGroup __instance, int optionIndex, string folderName)
    {
        var container = __instance._options[optionIndex].transform.Find("BounceContainer/Background");
        var barInstance = container.Find("ProgressBar(Clone)");

        // Debug
        // if (barInstance)
        //     UnityEngine.Object.Destroy(barInstance);

        if (!barInstance)
        {
            if (!ProgressBar.Template)
                ProgressBar.InitTemplate(container.parent.Find("LetterGradeDashed"));
            barInstance = UnityEngine.Object.Instantiate(ProgressBar.Template).transform;
            barInstance.transform.SetParent(container, worldPositionStays: false);
            barInstance.gameObject.SetActive(true);
        }

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

        ProgressBar.SetPercentage(barInstance.gameObject, Math.Round(FCedTracks / totalTracksInFolder, 2));

    }

    const int MaxBarWidth = 300;
    const int MaxBarHeight = 15;
    const int InnerBarMargin = 5;
    static readonly Color OuterBarColor = new Color(0.553f, 0.533f, 0.592f);
    static readonly Color InnerBarColor = Color.black;
    static readonly Vector3 BarPosition = new Vector3(250, -40, 0);
    static readonly Vector3 NumberPosition = new Vector3(50, -40, 0);

    static void InitTemplate(Transform textTemplate)
    {
        // TODO: make color change on select (likely something to do with TrackSelectionOptionColorAnimator)
        Template = new GameObject("ProgressBar", typeof(RectTransform));
        Template.SetActive(false);

        var outerBar = new GameObject("outerBar");
        outerBar.transform.SetParent(Template.transform);
        outerBar.transform.localPosition = BarPosition;
        outerBar.AddComponent<Image>().color = OuterBarColor;
        outerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(MaxBarWidth, MaxBarHeight);

        var innerBar = UnityEngine.Object.Instantiate(outerBar);
        innerBar.name = "innerBar";
        innerBar.GetComponent<Image>().color = InnerBarColor;
        innerBar.transform.SetParent(outerBar.transform);

        var number = UnityEngine.Object.Instantiate(textTemplate).gameObject;
        number.name = "number";
        number.transform.SetParent(Template.transform, false);
        number.transform.localPosition = NumberPosition;
        number.SetActive(true);
    }

    static void SetPercentage(GameObject bar, double percent)
    {
        var innerBar = bar.transform.Find("outerBar/innerBar");
        var innerBarWidth = MaxBarWidth * (float)percent;
        innerBar.GetComponent<RectTransform>().sizeDelta = new Vector2(innerBarWidth, MaxBarHeight - InnerBarMargin);
        innerBar.transform.localPosition = new Vector3(-MaxBarWidth / 2 + innerBarWidth / 2 + InnerBarMargin / 2, 0, 0);

        var number = bar.transform.Find("number");
        number.GetComponent<TextMeshProUGUI>().text = ((int)(percent * 100)).ToString() + "%";
        number.transform.localPosition = NumberPosition;
    }
}