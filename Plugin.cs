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
        const string PLUGIN_GUID = "dopadream.lethalcompany.LLLHotreloadPatch", PLUGIN_NAME = "LLLHotreloadPatch", PLUGIN_VERSION = "1.0.1";
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
            Plugin.Logger.LogInfo($"whatever currentClientId is: {NetworkManager.Singleton.LocalClientId}");
            Plugin.Logger.LogInfo($"Current moon's scene name: {RoundManager.Instance.currentLevel.sceneName}");
            Plugin.Logger.LogInfo($"AssetBundleInfos count: {LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos.Count})");
            foreach (var assetBundleInfo in LethalLevelLoader.AssetBundles.AssetBundleLoader.instance.AssetBundleInfos)
            {
                if (assetBundleInfo.assetBundle != null)
                {
                    Plugin.Logger.LogDebug($"AssetBundle: {assetBundleInfo.assetBundle}");
                }
                if (!assetBundleInfo.sceneNames.Contains(RoundManager.Instance.currentLevel.sceneName)) continue;
                Plugin.Logger.LogDebug($"I'm loading this bundle!");
                assetBundleInfo.TryLoadBundle();
                /*foreach (string sceneName in assetBundleInfo.sceneNames)
                {
                    Plugin.Logger.LogInfo($"Bundle contains this scene: {sceneName}");
                    if (sceneName.ToLowerInvariant != RoundManager.Instance.currentLevel.sceneName.ToLowerInvariant) continue;
                    Plugin.Logger.LogInfo($"I'm loading this bundle!");
                    assetBundleInfo.TryLoadBundle();
                    break;
                }*/
            }
        }
    }
}