using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.InputSystem.XR;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(XRLayoutBuilder), "Build")]
internal class XRLayoutBuilderBuild
{
    private static readonly MethodInfo _stringToLowerMethod =
        AccessTools.DeclaredMethod(typeof(string), nameof(string.ToLower), Array.Empty<Type>());
    private static readonly MethodInfo _stringToLowerInvariantMethod =
        AccessTools.DeclaredMethod(typeof(string), nameof(string.ToLowerInvariant));

    protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(i => i.Calls(_stringToLowerMethod)))
            .ThrowIfInvalid($"Couldn't find string.ToLower (call/callvirt)")
            .SetOperandAndAdvance(_stringToLowerInvariantMethod)
            .InstructionEnumeration();
    }
}
