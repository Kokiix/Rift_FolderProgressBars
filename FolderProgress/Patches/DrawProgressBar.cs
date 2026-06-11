
using HarmonyLib;
using Shared.TrackData;
using Shared.TrackSelection;
using UnityEngine;

[HarmonyPatch(typeof(BaseTrackSelectionOptionGroup), "InitializeTrackOption")]
static class DrawProgressBar
{
    static void Postfix(ITrackMetadata newTrackMetadata)
    {
        if (newTrackMetadata is FolderTrackMetadata folderMeta)
            Debug.LogError("folder object created: " + folderMeta.TrackName);
    }
}