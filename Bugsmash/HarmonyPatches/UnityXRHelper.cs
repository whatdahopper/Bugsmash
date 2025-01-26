using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Bugsmash.HarmonyPatches;

[HarmonyPatch(typeof(UnityXRHelper), nameof(UnityXRHelper.GetRootPositionOffsetForLegacyNodePose), typeof(UnityXRHelper.VRControllerManufacturerName))]
internal class UnityXRHelperGetRootPositionOffsetForLegacyNodePose
{
    [HarmonyPrepare]
    // Only active if OpenXR runtime is SteamVR
    private static bool Prepare() => OpenXRRuntime.name.IndexOf("steamvr", StringComparison.OrdinalIgnoreCase) >= 0;

    [HarmonyPostfix]
    private static void Postfix(UnityXRHelper.VRControllerManufacturerName manufacturerName, ref Pose __result)
    {
        if (manufacturerName == UnityXRHelper.VRControllerManufacturerName.Oculus)
        {
            __result = new Pose(new Vector3(0.007f, -0.0341572f, 0.09607324f), new Quaternion(0.1788023f, 0, 0, 0.983885f));
        }
    }
}

[HarmonyPatch(typeof(UnityXRHelper), nameof(UnityXRHelper.TryGetLegacyPoseOffsetForNode),
    new[] { typeof(UnityXRHelper.VRControllerManufacturerName), typeof(Vector3), typeof(Vector3) },
    new[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
internal class UnityXRHelperTryGetLegacyPoseOffsetForNode
{
    [HarmonyPrepare]
    // Only active if OpenXR runtime is SteamVR
    private static bool Prepare() => OpenXRRuntime.name.IndexOf("steamvr", StringComparison.OrdinalIgnoreCase) >= 0;

    [HarmonyPostfix]
    private static void Postfix(UnityXRHelper.VRControllerManufacturerName manufacturerName, ref Vector3 position, ref Vector3 rotation, ref bool __result)
    {
        if (manufacturerName == UnityXRHelper.VRControllerManufacturerName.Oculus)
        {
            __result = true;
            // it is the same as the HTC values but in case they decide to change it for HTC, it is hardcoded here
            position = new Vector3(0f, -0.008f, 0f);
            rotation = new Vector3(-4.3f, 0f, 0f);
        }
    }
}