using HarmonyLib;
using IPA;
using System.Reflection;
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

    internal static Harmony HarmonyInstance { get; } = new Harmony("com.github.whatdahopper.Bugsmash");

    [Init]
    public Plugin(IPALogger logger)
    {
        // ...
    }

    [OnEnable]
    public void OnEnable() => HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

    [OnDisable]
    public void OnDisable() => HarmonyInstance.UnpatchSelf();
}
