using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputPointable : MonoBehaviour, IPointable
{
    private InputField inputField;

    private void Start()
    {
        this.inputField = GetComponent<InputField>();
    }

    public void OnPointerClick(object sender, PointerEventArgs e)
    {
        this.inputField.OnSelect(null);
    }

    public void OnPointerInside(object sender, PointerEventArgs e)
    {
        
    }

    public void OnPointerOutside(object sender, PointerEventArgs e)
    {
        
    }
}
