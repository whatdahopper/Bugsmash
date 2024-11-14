using Bugsmash.HarmonyPatches;
using HarmonyLib;
using IPA;
using IPA.Loader;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace Bugsmash;

[Plugin(RuntimeOptions.DynamicInit)]
public class Plugin
{
#if DEBUG
    internal const bool IsDebugBuild = true;
#else
    internal const bool IsDebugBuild = false;
#endif
    internal const string CompatibleGameVersion = "1.39.0_1012";

    internal static bool IsCompatible { get; private set; }
    internal static bool IsCustomJSONDataInstalled { get; private set; }
    internal static Harmony HarmonyInstance { get; } = new Harmony("com.github.whatdahopper.Bugsmash");
    internal static IPALogger Log { get; private set; } = null!;

    [Init]
    public Plugin(IPALogger logger)
    {
        Log = logger;

        string gameVersion = Application.version;
        if (gameVersion != CompatibleGameVersion)
        {
            Log.Error($"Game version \"{gameVersion}\" is not compatible, expected: \"{CompatibleGameVersion}\".");
            return;
        }

        IsCompatible = true;
    }

    [OnEnable]
    public void OnEnable()
    {
        if (!IsCompatible)
            return;

        IsCustomJSONDataInstalled = PluginManager.GetPluginFromId("CustomJSONData") != null;

        Log.Info($"IsCompatible: {IsCompatible}, IsCustomJSONDataInstalled: {IsCustomJSONDataInstalled}");

        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        if (!IsCustomJSONDataInstalled)
        {
            MethodInfo getBeatmapDataFromSaveDataMethod = typeof(BeatmapDataLoaderVersion2_6_0AndEarlier.BeatmapDataLoader)
                .GetMethod("GetBeatmapDataFromSaveData", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo transpilerMethod = typeof(BeatmapDataLoaderGetBeatmapDataFromSaveData)
                .GetMethod("Transpiler", BindingFlags.Static | BindingFlags.NonPublic);
            HarmonyInstance.Patch(getBeatmapDataFromSaveDataMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }
    }

    [OnDisable]
    public void OnDisable()
    {
        if (!IsCompatible)
            return;

        HarmonyInstance.UnpatchSelf();
    }
}
