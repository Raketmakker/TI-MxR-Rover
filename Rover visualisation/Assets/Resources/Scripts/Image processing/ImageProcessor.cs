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
    private List<Texture2D> origialTextures;
    [Range(1, 100)]
    public int pixelSkips = 100;
    public float imageRecordInterval = 1;
    [Range(0.0f, 1.0f)]
    public float minimumDifference = 0.3f;
    public List<Texture2D> testList = new List<Texture2D>();

    async void Start()
    {
        this.origialTextures = new List<Texture2D>();
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

        //this.origialTextures.Add(testList[0]);
        //for (int i = 1; i < testList.Count; i++)
        //{
        //    if (await CompareTextures(testList[i]))
        //    {
        //        origialTextures.Add(testList[i]);
        //    }
        //}
        //SpawnTextures(origialTextures);
    }

    //TODO get the videoclip
    private VideoClip GetVideoClip(VideoPlayer videoPlayer)
    {
        return videoPlayer.clip;
    }

    private async void ParseFrame(VideoPlayer source, long frameIndex)
    {
        Texture2D clonedTex = CopyTexture(source);
        bool originalPicture = this.origialTextures.Count == 0 ? true : await CompareTextures(clonedTex);
        if(originalPicture)
            this.origialTextures.Add(clonedTex);
        if(frameIndex + (long) frameInterval < (long)source.frameCount)
        {
            source.frame = frameIndex + (long)frameInterval;
            return;
        }
        source.frameReady -= ParseFrame;        
        SpawnTextures(this.origialTextures);
    }

    private float CalculateFrameInterval(VideoPlayer source)
    {
        return (float)source.clip.frameRate * this.imageRecordInterval;
    }

    private Texture2D CopyTexture(VideoPlayer source)
    {
        Texture2D clonedTexture = new Texture2D(source.texture.width, source.texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(source.texture.width, source.texture.height, 32);
        Graphics.Blit(source.texture, renderTexture);
        RenderTexture.active = renderTexture;
        clonedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        clonedTexture.Apply();
        RenderTexture.active = currentRT;
        return clonedTexture;
    }

    private async Task<bool> CompareTextures(Texture2D texToCompare)
    {
        var tasks = new List<Task<bool>>();
        foreach(Texture2D originalTex in this.origialTextures)
        {
            tasks.Add(CompareTextureAsync(originalTex, texToCompare));
        }
        bool[] originals = await Task.WhenAll(tasks);
        foreach (bool value in originals)
        {
            if(value == false)
            {
                return false;
            }
        }
        return true;
    }

    //Compares grey value per pixel. Returns true if its a different picture
    private async Task<bool> CompareTextureAsync(Texture2D originalTex, Texture2D texToCompare)
    {
        int differenceCounter = 0;
        Color32[] originalColors = originalTex.GetPixels32();
        Color32[] colorsToCompare = texToCompare.GetPixels32();

        for (int i = 0; i < originalTex.width * originalTex.height; i += pixelSkips)
        {
            differenceCounter += Mathf.Abs(originalColors[i].r - colorsToCompare[i].r);
            differenceCounter += Mathf.Abs(originalColors[i].g - colorsToCompare[i].g);
            differenceCounter += Mathf.Abs(originalColors[i].b - colorsToCompare[i].b);
        }
        float threashold =  minimumDifference * ((originalTex.width * originalTex.height) / pixelSkips) * 3 * 255;
        Debug.Log("Is original: " + (differenceCounter > threashold) + ". Difference counter: " + differenceCounter + ". Theashold: " + threashold);
        return differenceCounter > threashold;
    }

    private void SpawnTextures(List<Texture2D> textures)
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
