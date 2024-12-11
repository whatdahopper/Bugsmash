using HarmonyLib;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where GradientSkybox based on color scheme is using default colors.
// Currently, we are restoring the original behavior, by ignoring didChangeColorEvent, as it's not even invoked.

[HarmonyPatch(typeof(BloomPrePassBackgroundColorsGradientFromColorSchemeColors), "Start")]
internal class BloomPrePassBackgroundColorsGradientFromColorSchemeColorsStart
{
    protected static bool Prefix(BloomPrePassBackgroundColorsGradientFromColorSchemeColors __instance)
    {
        __instance.HandleColorProviderDidChangeColor();
        return false;
    }
}

[HarmonyPatch(typeof(BloomPrePassBackgroundColorsGradientFromColorSchemeColors), "OnDestroy")]
internal class BloomPrePassBackgroundColorsGradientFromColorSchemeColorsOnDestroy
{
    protected static bool Prefix() => false;
}
