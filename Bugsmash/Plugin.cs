using HarmonyLib;
using IPA;
using System.Reflection;

namespace Bugsmash;

[Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
public class Plugin
{
    [Init]
    public Plugin() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.github.whatdahopper.Bugsmash");
}
