using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VideoItemCreator : MonoBehaviour
{
    private string path;
    private List<string> videoPaths;
    public GameObject spawnPanel;
    public string videoExtention = ".mp4";
    public string subfolder = "Rover visualisation";

    private void OnEnable()
    {
        this.videoPaths = new List<string>();
        if (!FolderHasVideos(ref videoPaths))
            return;

        CreateVideoPanels();
    }

    private bool FolderHasVideos(ref List<string> videos)
    {
        this.path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + this.subfolder;
        if (!Directory.Exists(this.path))
        {
            Directory.CreateDirectory(this.path);
            Debug.LogWarning("Created video directory at: " + this.path);
            return false;
        }

        string[] files = Directory.GetFiles(this.path);
        if (!(files.Length > 0))
        {
            Debug.LogWarning("No videos found at: " + this.path);
            return false;
        }

        bool hasVideos = false;
        foreach (var file in files)
        {
            if (Path.GetExtension(file).Equals(this.videoExtention))
            {
                videos.Add(file);
                hasVideos = true;
            }
        }
        return hasVideos;
    }

    public void CreateVideoPanels()
    {
        Debug.Log("Following videos were found: " + string.Join(",", this.videoPaths));

        foreach (var videoPath in this.videoPaths)
        {
            GameObject videoPanel = Instantiate(this.spawnPanel, this.transform);
            VideoItem item = videoPanel.GetComponent<VideoItem>();
            item.Init(videoPath);
        }
    }
}
