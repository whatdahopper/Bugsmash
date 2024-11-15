using Bugsmash.Utilities;
using HarmonyLib;
using System.Collections.Generic;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(LightRotationEventEffect), "Start")]
internal class LightRotationEventEffectStart
{
    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => PatchUtil.SetRandomSeed(instructions);
}
