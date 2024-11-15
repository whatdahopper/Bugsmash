using Bugsmash.Utilities;
using HarmonyLib;
using System.Collections.Generic;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(TrackLaneRingsRotationEffectSpawner), "Start")]
internal class TrackLaneRingsRotationEffectSpawnerStart
{
    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => PatchUtil.SetRandomSeed(instructions);
}
