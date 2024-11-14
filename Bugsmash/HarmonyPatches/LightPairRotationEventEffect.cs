using Bugsmash.Utilities;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(LightPairRotationEventEffect), "Start")]
internal class LightPairRotationEventEffectStart
{
    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = instructions.ToList();
        PatchUtil.FixRandomSeed(ref codes);
        return codes;
    }
}
