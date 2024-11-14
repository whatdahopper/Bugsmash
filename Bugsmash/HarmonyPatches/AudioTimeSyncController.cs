using HarmonyLib;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where v2 lights do not pause when pausing the beatmap.

[HarmonyPatch(typeof(AudioTimeSyncController), "Update")]
internal class AudioTimeSyncControllerUpdate
{
    protected static bool Prefix(AudioTimeSyncController __instance)
    {
        if (__instance._state == AudioTimeSyncController.State.Paused ||
            __instance._state == AudioTimeSyncController.State.Stopped)
        {
            __instance._lastFrameDeltaSongTime = 0f;
            return false;
        }
        return true;
    }
}
