/* SceneHandler.cs*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserPointer : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        Button b = e.target.GetComponent<Button>();
        b?.OnSubmit(null);
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Button b = e.target.GetComponent<Button>();
        b?.OnSelect(null);
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Button b = e.target.GetComponent<Button>();
        b?.OnDeselect(null);
    }
}