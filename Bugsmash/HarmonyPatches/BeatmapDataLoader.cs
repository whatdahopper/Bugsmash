using HarmonyLib;
using IPA.Loader;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// This patch was shamelessly copied from CustomJSONData, thanks to Aeroluna for it:
// https://github.com/Aeroluna/CustomJSONData/blob/master/CustomJSONData/HarmonyPatches/CustomBeatmapData/FixV2_6_0AndEarlierDefaultEnvironmentEvents.cs
// It is conditionally installed, as CustomJSONData also has this patch.

[HarmonyPatch(typeof(BeatmapDataLoaderVersion2_6_0AndEarlier.BeatmapDataLoader), "GetBeatmapDataFromSaveData")]
internal class BeatmapDataLoaderGetBeatmapDataFromSaveData
{
    private static readonly MethodInfo _insertDefaultEventsMethod =
        AccessTools.Method(typeof(DefaultEnvironmentEventsFactory), "InsertDefaultEvents");
    private static readonly MethodInfo _insertDefaultEventsConditionalMethod =
        SymbolExtensions.GetMethodInfo(() => InsertDefaultEventsConditional(null!, false));

    protected static bool Prepare() => PluginManager.GetPluginFromId("CustomJSONData") == null;

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Call, _insertDefaultEventsMethod))
            .ThrowIfInvalid("Couldn't find InsertDefaultEvents (call)")
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .SetOperandAndAdvance(_insertDefaultEventsConditionalMethod)
            .InstructionEnumeration();
    }

    private static void InsertDefaultEventsConditional(BeatmapData beatmapData, bool flag3)
    {
        if (!flag3)
            DefaultEnvironmentEventsFactory.InsertDefaultEvents(beatmapData);
    }
}
