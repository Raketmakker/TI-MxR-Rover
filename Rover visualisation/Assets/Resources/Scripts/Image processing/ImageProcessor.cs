using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class ImageProcessor : MonoBehaviour
{
    private float frameInterval;
    private List<Texture> textures;
    public float imageRecordInterval = 1;
    public delegate void ImageProcessed();
    public event ImageProcessed OnImageProcessed;

    void Start()
    {
        this.textures = new List<Texture>();
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = GetVideoClip(videoPlayer);
        this.frameInterval = CalculateFrameInterval(videoPlayer);
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += (VideoPlayer source) =>
        {
            source.Pause();
        };
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += ParseFrame; 
        videoPlayer.Prepare();
    }

    //TODO get the videoclip
    private VideoClip GetVideoClip(VideoPlayer videoPlayer)
    {
        return videoPlayer.clip;
    }

    private void ParseFrame(VideoPlayer source, long frameIndex)
    {
        this.textures.Add(CopyTexture(source));
        if(frameIndex + (long) frameInterval > (long)source.frameCount)
        {
            source.frameReady -= ParseFrame;
            SpawnTextures();
        }
        else
        {
            source.frame = frameIndex + (long)frameInterval;
        }
    }

    private float CalculateFrameInterval(VideoPlayer source)
    {
        return (float)source.clip.frameRate * this.imageRecordInterval;
    }

    private Texture CopyTexture(VideoPlayer source)
    {
        Texture tex = new Texture2D(source.texture.width, source.texture.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(source.texture, tex);
        return tex;
    }

    public void SpawnTextures()
    {
        Debug.LogWarning("ImageProcessor.SpawnTextures has to be removed!");
        for (int i = 0; i < textures.Count; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            Material newMat = new Material(renderer.material);
            newMat.mainTexture = textures[i];
            renderer.material = newMat;
            cube.transform.position = new Vector3(i * 10, 0, 0);
        }
    }
}
