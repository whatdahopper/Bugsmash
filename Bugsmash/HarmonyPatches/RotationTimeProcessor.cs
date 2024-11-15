using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where v2 rotation events are not handled correctly.

[HarmonyPatch(typeof(RotationTimeProcessor), MethodType.Constructor, typeof(IReadOnlyList<BeatmapSaveDataVersion2_6_0AndEarlier.EventData>))]
internal class RotationTimeProcessorConstructor
{
    private static readonly MethodInfo _getValueMethod =
        AccessTools.PropertyGetter(typeof(BeatmapSaveDataVersion2_6_0AndEarlier.EventData), "value");
    private static readonly MethodInfo _getFloatValueMethod =
        AccessTools.PropertyGetter(typeof(BeatmapSaveDataVersion2_6_0AndEarlier.EventData), "floatValue");
    private static readonly ConstructorInfo _rotationChangeDataConstructor =
        AccessTools.Constructor(typeof(RotationTimeProcessor.RotationChangeData), new[] { typeof(float), typeof(int) });
    private static readonly MethodInfo _spawnRotationForEventValueMethod =
        SymbolExtensions.GetMethodInfo(() => SpawnRotationForEventValue(0));
    private static readonly float[] _spawnRotations = new[] { -60f, -45f, -30f, -15f, 15f, 30f, 45f, 60f };

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Callvirt, _getFloatValueMethod))
            .ThrowIfInvalid("Couldn't find get_floatValue (callvirt)")
            // Replace with proper rotation value.
            .Repeat(m =>
            {
                m.SetOperandAndAdvance(_getValueMethod)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, _spawnRotationForEventValueMethod));
            })
            .End()
            // Convert to integer (from float) and add.
            .MatchBack(false, new CodeMatch(OpCodes.Call, _spawnRotationForEventValueMethod))
            .ThrowIfInvalid("Couldn't find SpawnRotationForEventValue (call)")
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Conv_I4), new CodeInstruction(OpCodes.Add))
            .SetAndAdvance(OpCodes.Stloc_0, null)
            // Load local integer rotation value.
            .MatchBack(false, new CodeMatch(OpCodes.Ldloc_1), new CodeMatch(OpCodes.Ldloc_2))
            .ThrowIfInvalid("Couldn't find ldloc.1 and ldloc.2")
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            // Lets use it when constructing RotationChangeData.
            .MatchForward(false, new CodeMatch(OpCodes.Newobj, _rotationChangeDataConstructor))
            .ThrowIfInvalid("Couldn't find RotationChangeData (newobj)")
            // Remove integer conversion from here, as we no longer need it.
            .Advance(-1)
            .RemoveInstruction()
            // Replace ldloc.s with ldloc.0 (the local integer rotation value).
            .Advance(-1)
            .SetAndAdvance(OpCodes.Ldloc_0, null)
            .InstructionEnumeration();
    }

    private static float SpawnRotationForEventValue(int index)
        => index >= 0 && index < _spawnRotations.Length ? _spawnRotations[index] : 0f;
}
