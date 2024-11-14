using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// This patch was shamelessly copied from CustomJSONData, thanks to Aeroluna for it:
// https://github.com/Aeroluna/CustomJSONData/blob/master/CustomJSONData/HarmonyPatches/CustomBeatmapData/FixV2_6_0AndEarlierDefaultEnvironmentEvents.cs
// It is conditionally installed, as CustomJSONData also has this patch.

internal class BeatmapDataLoaderGetBeatmapDataFromSaveData
{
    private static readonly MethodInfo _insertDefaultEvents =
        AccessTools.Method(typeof(DefaultEnvironmentEventsFactory), nameof(DefaultEnvironmentEventsFactory.InsertDefaultEvents));
    private static readonly MethodInfo _insertDefaultEventsConditional =
        AccessTools.Method(typeof(BeatmapDataLoaderGetBeatmapDataFromSaveData), nameof(InsertDefaultEventsConditional));

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Call, _insertDefaultEvents))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .SetOperandAndAdvance(_insertDefaultEventsConditional)
            .InstructionEnumeration();
    }

    private static void InsertDefaultEventsConditional(BeatmapData beatmapData, bool flag3)
    {
        if (!flag3)
            DefaultEnvironmentEventsFactory.InsertDefaultEvents(beatmapData);
    }
}
