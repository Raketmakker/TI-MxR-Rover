using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Valve.VR;

public class VideoController : MonoBehaviour
{
    public SteamVR_Action_Boolean togglePlaying;
    public SteamVR_Action_Vector2 joystickAction;
    public VideoPlayer videoPlayer;
    public int framesToSkip = 60;
    public float threashold = 0.1f;

    void FixedUpdate()
    {
        if (DetectVR.VRController.none == DetectVR.GetControllerTypeToEnum())
        {
            float value = Input.GetKey(KeyCode.Keypad6) ? 1 : Input.GetKey(KeyCode.Keypad4) ? -1 : 0;
            Skip(value);
            if (Input.GetKeyUp(KeyCode.Keypad5))
                TogglePause();
        }
        else
        {
            Skip(joystickAction.GetAxis(SteamVR_Input_Sources.Any).x);
            if (this.togglePlaying.GetLastStateUp(SteamVR_Input_Sources.Any))
                TogglePause();
        }
    }

    private void TogglePause() 
    {
        if (this.videoPlayer.isPlaying)
            this.videoPlayer.Pause();
        else
            this.videoPlayer.Play();
    }

    private void Skip(float value)
    {
        if(value > this.threashold || value < -this.threashold)
            this.videoPlayer.frame = (long)Mathf.Clamp(this.videoPlayer.frame + value * this.framesToSkip, 0, this.videoPlayer.frameCount);
    }
}
