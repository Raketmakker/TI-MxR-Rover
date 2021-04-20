using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonPointable : MonoBehaviour, IPointable
{
    public void OnPointerClick(object sender, PointerEventArgs e)
    {
        GetComponent<Button>()?.OnSubmit(null);
    }

    public void OnPointerInside(object sender, PointerEventArgs e)
    {
        GetComponent<Button>()?.OnSelect(null);
    }

    public void OnPointerOutside(object sender, PointerEventArgs e)
    {
        GetComponent<Button>()?.OnDeselect(null);
    }
}
