using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

/// <summary>
/// These are a few different ways to detect the current VR HMD and controller type.
/// - Unity XR api
/// - Unity Input api
/// - SteamVR plugin
/// </summary>
public static class DetectVR
{
    /// <summary>
    /// Possible HMD types
    /// </summary>
    public enum VRHMD
    {
        vive,
        vive_pro,
        vive_cosmos,
        rift,
        indexhmd,
        holographic_hmd,
        none
    }


    /// <summary>
    /// Possible controller types
    /// </summary>
    public enum VRController
    {
        vive_controller,
        vive_cosmos_controller,
        oculus_touch,
        knuckles,
        holographic_controller,
        none
    }

    /// <summary>
    /// Get the HMD type using Unity XR/VR
    /// </summary>
    /// <returns>"vive_cosmos", "VIVE_Pro MV", "Vive Cosmos External Tracking Mod", "Samsung Windows Mixed Reality 800ZBA0"</returns>
    public static string GetHmdTypeToString()
    {
#if UNITY_2017_2_OR_NEWER
        return UnityEngine.XR.XRDevice.model;
#endif

#if UNITY_5_3_4_OR_NEWER
        return UnityEngine.VR.VRDevice.model
#endif
    }

    /// <summary>
    /// Get the HMD type using SteamVR plugin
    /// </summary>
    /// <returns>"vive_cosmos", "VIVE_Pro MV", "Vive Cosmos External Tracking Mod", "Samsung Windows Mixed Reality 800ZBA0"</returns>
    public static string GetHmdTypeSteamVRToString()
    {
        if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
            return Valve.VR.SteamVR.instance.hmd_ModelNumber;

        return null;
    }

    /// <summary>
    /// Get the VR controller type using Unity XR/VR
    /// </summary>
    /// <returns>Controller type as enum</returns>
    public static VRController GetControllerTypeToEnum()
    {
        // check XR nodes
        var nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);
        foreach (XRNodeState nodeState in nodeStates)
        {
            if (InputTracking.GetNodeName(nodeState.uniqueID).Contains("vive_controller"))
                return VRController.vive_controller;
            if (InputTracking.GetNodeName(nodeState.uniqueID).Contains("vive_cosmos_controller"))
                return VRController.vive_cosmos_controller;
            if (InputTracking.GetNodeName(nodeState.uniqueID).Contains("oculus_touch"))
                return VRController.oculus_touch;
            if (InputTracking.GetNodeName(nodeState.uniqueID).Contains("knuckles"))
                return VRController.knuckles;
            if (InputTracking.GetNodeName(nodeState.uniqueID).Contains("holographic_controller"))
                return VRController.holographic_controller;
        }
        return VRController.none;
    }

    /// <summary>
    /// Get the VR controller type using Unity XR/VR
    /// </summary>
    /// <returns>"OpenVR Controller(vive_cosmos_controller) - Left", "OpenVR Controller(Knuckles Left) - Left"</returns>
    public static string GetControllerTypeToString()
    {
        // check XR nodes
        var nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);
        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == XRNode.LeftHand || nodeState.nodeType == XRNode.RightHand)
                return InputTracking.GetNodeName(nodeState.uniqueID);
        }
        return null;
    }

    /// <summary>
    /// Get the VR controller type using Unity input joystick
    /// </summary>
    /// <returns>"OpenVR Controller(vive_cosmos_controller) - Left", "OpenVR Controller(Knuckles Left) - Left"</returns>
    public static string GetControllerType2ToString()
    {
        if (Input.GetJoystickNames().Length == 0)
            return null;

        return Input.GetJoystickNames()[0];
    }


    /// <summary>
    /// Get the VR controller type using SteamVR plugin
    /// </summary>
    /// <returns>"vive_cosmos_controller", "Knuckles Right", "VIVE Controller Pro MV", "WindowsMR: 0x045E/0x065D/0/1"</returns>
    public static string GetControllerTypeSteamVRToString()
    {
        if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeFailure)
            return null;

        var rightHand = OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
        var sb = new System.Text.StringBuilder((int)64);
        ETrackedPropertyError propError = ETrackedPropertyError.TrackedProp_UnknownProperty;
        OpenVR.System.GetStringTrackedDeviceProperty(rightHand, ETrackedDeviceProperty.Prop_ModelNumber_String, sb, 2000, ref propError);
        return sb.ToString();
    }
}