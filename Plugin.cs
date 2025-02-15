using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Unity.Netcode;

namespace LLLHotreloadPatch
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "dopadream.lethalcompany.LLLHotreloadPatch", PLUGIN_NAME = "LLLHotreloadPatch", PLUGIN_VERSION = "1.0.0";
        internal static new ManualLogSource Logger;

        void Awake()
        {
            Logger = base.Logger;

            new Harmony(PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"{PLUGIN_NAME} v{PLUGIN_VERSION} loaded");
        }
    }

    [HarmonyPatch]
    internal class LLLHotreloadPatches()
    {
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnLocalDisconnect))]
        [HarmonyPostfix]
        static void StartOfRoundOnLocalDisconnectPostfix()
        {
            Plugin.Logger.LogDebug($"whatever currentClientId is: {NetworkManager.Singleton.LocalClientId}");
            Plugin.Logger.LogDebug($"AssetBundleInfos count: {LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos.Count})");
            foreach (var assetBundleInfo in LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos)
            {
                Plugin.Logger.LogDebug($"AssetBundleInfo: {assetBundleInfo}");
                Plugin.Logger.LogDebug($"AssetBundle: {assetBundleInfo.assetBundle}");
                Plugin.Logger.LogDebug($"Active Loading Status: {assetBundleInfo.ActiveLoadingStatus}");
                assetBundleInfo.TryUnloadBundle();
            }
        }

        [HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.Start))]
        [HarmonyPostfix]
        static void JoinOrStartLobbyPostfix()
        {
            Plugin.Logger.LogDebug($"whatever currentClientId is: {NetworkManager.Singleton.LocalClientId}");
            Plugin.Logger.LogDebug($"AssetBundleInfos count: {LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos.Count})");
            foreach (var assetBundleInfo in LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos)
            {
                Plugin.Logger.LogDebug($"AssetBundleInfo: {assetBundleInfo}");
                Plugin.Logger.LogDebug($"AssetBundle: {assetBundleInfo.assetBundle}");
                Plugin.Logger.LogDebug($"Active Loading Status: {assetBundleInfo.ActiveLoadingStatus}");
                assetBundleInfo.TryLoadBundle();
            }
        }
    }
}