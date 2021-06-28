using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using Valve.VR;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollbarPointable : MonoBehaviour, IPointable
{
    private bool selected = false;
    private ScrollRect scrollRect;
    public float scrollSpeed = 0.1f;
    public SteamVR_Action_Vector2 joystickAction;

    private void Start()
    {
        this.scrollRect = GetComponent<ScrollRect>();
    }

    public void OnPointerClick(object sender, PointerEventArgs e)
    {
        
    }

    public void OnPointerInside(object sender, PointerEventArgs e)
    {
        this.selected = true;
    }

    public void OnPointerOutside(object sender, PointerEventArgs e)
    {
        this.selected = false;
    }

    void FixedUpdate()
    {
        if (this.joystickAction == null)
            return;

        Vector2 joystickValue = joystickAction.GetAxis(SteamVR_Input_Sources.Any);
        if (this.selected && joystickValue != Vector2.zero)
        {
            this.scrollRect.verticalNormalizedPosition = Mathf.Clamp01(this.scrollRect.verticalNormalizedPosition + joystickValue.y * Time.fixedDeltaTime * scrollSpeed);
        }
    }
}
