using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where color schemes are getting the incorrect arguments when constructing the class.
// This should be checked again, and made sure that when this is fixed, that this patch is remove as it could corrupt color schemes!

[HarmonyPatch(typeof(PlayerDataFileModel), "GetPlayerSaveData")]
internal class PlayerDataFileModelGetPlayerSaveData
{
    private static readonly MethodInfo _getObstaclesColorMethod =
        AccessTools.Method(typeof(ColorScheme), "get_obstaclesColor");
    private static readonly MethodInfo _getEnvironmentColor0Method =
        AccessTools.Method(typeof(ColorScheme), "get_environmentColor0");
    private static readonly MethodInfo _getEnvironmentColor1Method =
        AccessTools.Method(typeof(ColorScheme), "get_environmentColor1");

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _getObstaclesColorMethod))
            .ThrowIfInvalid("Couldn't find get_obstaclesColor (callvirt)")
            .SetOperandAndAdvance(_getEnvironmentColor0Method)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _getEnvironmentColor0Method))
            .ThrowIfInvalid("Couldn't find get_environmentColor0 (callvirt)")
            .SetOperandAndAdvance(_getEnvironmentColor1Method)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _getEnvironmentColor1Method))
            .ThrowIfInvalid("Couldn't find get_environmentColor1 (callvirt)")
            .SetOperandAndAdvance(_getObstaclesColorMethod)
            .InstructionEnumeration();
    }
}
