using BeatmapSaveDataVersion2_6_0AndEarlier;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.HarmonyPatches;

// Resolves an issue where v2 rotation events are not handled correctly.

[HarmonyPatch(typeof(RotationTimeProcessor), MethodType.Constructor, typeof(IReadOnlyList<EventData>))]
internal class RotationTimeProcessorConstructor
{
    private const string GetValueMethodName = "get_value";
    private const string GetFloatValueMethodName = "get_floatValue";
    private static readonly MethodInfo _getValueMethod = typeof(EventData).GetMethod(GetValueMethodName);
    private static readonly MethodInfo _spawnRotationForEventValueMethod = SymbolExtensions.GetMethodInfo(() => SpawnRotationForEventValue(0));
    private static readonly FieldInfo _rotationChangeDataListField = typeof(RotationTimeProcessor).GetField("_rotationChangeDataList", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly float[] _spawnRotations = new float[8] { -60f, -45f, -30f, -15f, 15f, 30f, 45f, 60f };

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = instructions.ToList();
        for (int i = 0; i < codes.Count - 1; i++)
        {
            if (codes[i].Is(OpCodes.Ldfld, _rotationChangeDataListField))
            {
                codes.RemoveRange(i + 2, 2);
                codes.Insert(i + 2, new(OpCodes.Ldloc_0));

                int count = 0;
                for (int c = i; c >= 0; c--)
                {
                    if (codes[c].opcode == OpCodes.Callvirt && ((MethodInfo)codes[c].operand).Name == GetFloatValueMethodName)
                    {
                        // Make some changes, we need to get the proper spawn rotation instead of using float value
                        List<CodeInstruction> toInsert = new()
                        {
                            new(OpCodes.Callvirt, _getValueMethod),
                            new(OpCodes.Call, _spawnRotationForEventValueMethod)
                        };
                        if (count == 0)
                        {
                            toInsert.Add(new(OpCodes.Conv_I4));
                            toInsert.Add(new(OpCodes.Add));
                            toInsert.Add(new(OpCodes.Stloc_0));

                            codes.RemoveAt(c + 1);
                            codes.Insert(c - 3, new(OpCodes.Ldloc_0));
                            c++;
                        }

                        codes.RemoveAt(c);
                        codes.InsertRange(c, toInsert);
                        count++;
                    }
                }
                break;
            }
        }
        return codes;
    }

    private static float SpawnRotationForEventValue(int index)
    {
        if (index >= 0 && index < _spawnRotations.Length)
            return _spawnRotations[index];
        return 0f;
    }
}
