using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.Utilities;

internal class PatchUtil
{
    private static readonly ConstructorInfo _randomConstructor =
        AccessTools.Constructor(typeof(System.Random), new[] { typeof(int) });
    private static readonly MethodInfo _getInstanceIDMethod =
        AccessTools.Method(typeof(UnityEngine.Object), "GetInstanceID");

    public static IEnumerable<CodeInstruction> SetRandomSeed(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_0), new CodeMatch(OpCodes.Newobj, _randomConstructor))
            .ThrowIfInvalid("Couldn't find System.Random constructor with default seed value of 0")
            .SetOpcodeAndAdvance(OpCodes.Ldarg_0)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, _getInstanceIDMethod))
            .InstructionEnumeration();
    }
}
