using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Bugsmash.Utilities;

internal class PatchUtil
{
    private static readonly ConstructorInfo _randomCtor =
        typeof(System.Random).GetConstructor(new[] { typeof(int) });
    private static readonly MethodInfo _getInstanceID =
        typeof(UnityEngine.Object).GetMethod(nameof(UnityEngine.Object.GetInstanceID));

    public static void FixRandomSeed(ref List<CodeInstruction> codes)
    {
        for (int i = 0; i < codes.Count - 1; i++)
        {
            if (codes[i].Is(OpCodes.Newobj, _randomCtor) && codes[i - 1].opcode == OpCodes.Ldc_I4_0)
            {
                codes.RemoveAt(i - 1); // Remove the default seed value of 0
                codes.InsertRange(i - 1, new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, _getInstanceID)
                });
            }
        }
    }
}
