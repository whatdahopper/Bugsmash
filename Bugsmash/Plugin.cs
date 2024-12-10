using HarmonyLib;
using IPA;
using System.Reflection;

namespace Bugsmash;

[Plugin(RuntimeOptions.SingleStartInit)]
public class Plugin
{
    internal static Harmony HarmonyInstance { get; } = new("com.github.whatdahopper.Bugsmash");

    [OnStart]
    public void OnStart() => HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

    [OnExit]
    public void OnExit() => HarmonyInstance.UnpatchSelf();
}
