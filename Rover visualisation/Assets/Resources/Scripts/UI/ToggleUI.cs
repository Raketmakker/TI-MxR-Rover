using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class ToggleUI : MonoBehaviour
{
    public SteamVR_Action_Boolean menuAction;
    public GameObject userInterface;

    private void Update()
    {
        if (this.menuAction == null)
            return;

        if (this.menuAction.GetStateUp(SteamVR_Input_Sources.Any))
        {
            this.userInterface.SetActive(!this.userInterface.activeInHierarchy);
        }
    }
}
