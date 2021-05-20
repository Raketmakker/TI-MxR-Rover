using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class ToggleUI : MonoBehaviour
{
    public SteamVR_Action_Boolean menuAction;
    public GameObject userInterface;

    private void Awake()
    {
        if (DetectVR.VRController.none == DetectVR.GetControllerTypeToEnum())
            this.enabled = false;
    }

    private void Update()
    {
        if (this.menuAction == null)
            return;

        if (this.menuAction.GetLastStateUp(SteamVR_Input_Sources.Any))
        {
            this.userInterface.SetActive(!this.userInterface.activeInHierarchy);
        }
    }
}
