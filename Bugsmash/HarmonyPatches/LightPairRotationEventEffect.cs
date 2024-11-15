using Bugsmash.Utilities;
using HarmonyLib;
using System.Collections.Generic;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(LightPairRotationEventEffect), "Start")]
internal class LightPairRotationEventEffectStart
{
    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        => PatchUtil.SetRandomSeed(instructions);
}
