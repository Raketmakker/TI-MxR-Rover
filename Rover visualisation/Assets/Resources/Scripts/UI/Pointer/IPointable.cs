using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public interface IPointable
{
    void OnPointerInside(object sender, PointerEventArgs e);
    void OnPointerOutside(object sender, PointerEventArgs e);
    void OnPointerClick(object sender, PointerEventArgs e);
}
