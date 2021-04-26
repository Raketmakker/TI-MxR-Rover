using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VideoItemCreator : ScrollViewItemCreator
{
    private List<string> videos;
    public string videoExtention = ".mp4";
    public string subfolder = "Rover visualisation";

    private bool FolderHasVideos(ref List<string> videos)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\" + this.subfolder;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.LogWarning("Created video directory at: " + path);
            return false;
        }

        string[] files = Directory.GetFiles(path);
        if (!(files.Length > 0))
        {
            Debug.LogWarning("No videos found at: " + path);
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

    public override List<GameObject> CreateItems()
    {
        this.videos = new List<string>();
        if (!FolderHasVideos(ref this.videos))
        {
            return new List<GameObject>();
        }
            Debug.Log("Following videos were found: " + string.Join(",", this.videos));
        return new List<GameObject>();
    }
}
