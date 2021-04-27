using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoHelper : MonoBehaviour
{
    private VideoPlayer player;
    public string dataTag = "Data";

    private void Awake()
    {
        this.player = GetComponent<VideoPlayer>();
    }
    private void Start()
    {
        GameObject dataholder = GameObject.FindWithTag(this.dataTag);
        VideoData data = dataholder.GetComponent<VideoData>();
        this.player.url = data.VideoPath;
        this.player.Play();
        Destroy(dataholder);
    }
}
