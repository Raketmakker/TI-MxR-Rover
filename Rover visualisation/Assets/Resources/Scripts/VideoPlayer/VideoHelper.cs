using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHelper : MonoBehaviour
{
    public VideoPlayer player;
    public Text videoName;
    public Text videoDate;
    public string dataTag = "Data";

    private void Start()
    {
        GameObject dataholder = GameObject.FindWithTag(this.dataTag);
        if (dataholder == null)
            return;
        VideoData data = dataholder.GetComponent<VideoData>();
        FileInfo info = new FileInfo(data.VideoPath);
        this.player.url = data.VideoPath;
        this.player.Play();
        this.videoName.text = info.Name;
        this.videoDate.text = info.CreationTime.ToString();
        Destroy(dataholder);
    }
}
