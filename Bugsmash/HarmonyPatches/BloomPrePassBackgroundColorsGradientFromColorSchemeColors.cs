using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where GradientSkybox based on color scheme is using default colors.
// Currently, we are restoring the original behavior, by ignoring didChangeColorEvent, as it's not even invoked.

[HarmonyPatch(typeof(BloomPrePassBackgroundColorsGradientFromColorSchemeColors), "Start")]
internal class BloomPrePassBackgroundColorsGradientFromColorSchemeColorsStart
{
    private static readonly MethodInfo _handleColorProviderDidChangeColorMethod =
        AccessTools.Method(typeof(BloomPrePassBackgroundColorsGradientFromColorSchemeColors), "HandleColorProviderDidChangeColor");

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new List<CodeInstruction>()
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, _handleColorProviderDidChangeColorMethod),
            new(OpCodes.Ret)
        };
    }
}

[HarmonyPatch(typeof(BloomPrePassBackgroundColorsGradientFromColorSchemeColors), "OnDestroy")]
internal class BloomPrePassBackgroundColorsGradientFromColorSchemeColorsOnDestroy
{
    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        new List<CodeInstruction>() { new(OpCodes.Ret) };
}
