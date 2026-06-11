using BepInEx;
using FolderProgress;
using HarmonyLib;
using UnityEngine;

// [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
// [BepInDependency("com.lalabuff.necrodancer.necromanager", BepInDependency.DependencyFlags.SoftDependency)]
// class Plugin : RiftPlugin
// {
// }

// Non Necromanager version
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class FolderPPlugin : BaseUnityPlugin
{
    internal static Harmony harmony;

    void Awake()
    {
        harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }
    void OnDestroy()
    {
        harmony.UnpatchSelf();
    }
}