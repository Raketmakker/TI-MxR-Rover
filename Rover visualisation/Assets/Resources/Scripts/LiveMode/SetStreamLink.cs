using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetStreamLink : MonoBehaviour
{
    public GameObject errorMsg;
    public MediaPlayer mediaPlayer;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(Config.PerfKeys.livestreamAdres.ToString()))
        {
            bool opening = mediaPlayer.OpenMedia(
                new MediaPath(
                    PlayerPrefs.GetString(Config.PerfKeys.livestreamAdres.ToString()), 
                    MediaPathType.AbsolutePathOrURL));
            if (!opening)
            {
                errorMsg.SetActive(true);
            }
        }
    }
}
