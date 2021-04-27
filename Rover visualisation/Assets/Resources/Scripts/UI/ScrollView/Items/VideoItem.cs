using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VideoItem : MonoBehaviour
{
    private string path;
    public Text name;
    public Text date;
    public GameObject dataholder;
    public int nextNodeIndex;

    public void Init(string videoPath)
    {
        this.path = videoPath;
        FileInfo info = new FileInfo(videoPath);
        this.name.text = info.Name;
        this.date.text = info.CreationTime.ToString();
    }

    public void Delete()
    {
        FileInfo info = new FileInfo(this.path);
        info.Delete();
        this.enabled = false;
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }

    public void SetVideoData()
    {
        GameObject dataholder = Instantiate(this.dataholder);
        DontDestroyOnLoad(dataholder);
        VideoData data = dataholder.GetComponent<VideoData>();
        data.VideoPath = this.path;
    }

    public void LoadVideo()
    {
        UiNode parent = this.transform.parent.GetComponent<UiNode>();
        parent.SelectNode(this.nextNodeIndex);
    }
}
