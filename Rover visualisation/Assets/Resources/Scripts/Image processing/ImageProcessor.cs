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
    private RenderTexture renderTexture;
    [Range(1, 100)]
    public int pixelIncrement = 100;
    public float imageRecordInterval = 1;
    [Range(0.0f, 1.0f)]
    public float minimumDifference = 0.3f;
    public string dataTag = "Data";
    public Dictionary<Texture2D, Color32[]> textureColors;
    public event EventHandler<float> OnImageProgress;
    public event EventHandler OnFinishedParsing;

    /// <summary>
    /// Prepares the videoplayer for image extraction.
    /// Every extracted image will be compared to the previous images.
    /// If the new image is visable unique, it will be saved. 
    /// Else it will be discarted.
    /// </summary>
    async void Start()
    {
        this.textureColors = new Dictionary<Texture2D, Color32[]>();
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        if (!SetVideoPath(ref videoPlayer))
        {
            Debug.LogWarning("Could not find videoclip");
            return;
        }
        this.frameInterval = CalculateFrameInterval(videoPlayer);
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += (VideoPlayer source) =>
        {
            source.Pause();
            this.renderTexture = new RenderTexture(videoPlayer.texture.width, videoPlayer.texture.height, 32);
        };
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += ParseFrame;
        videoPlayer.Prepare();

#if UNITY_EDITOR
        OnImageProgress += (object o, float v) => Debug.Log("Parsing: " + (v * 100) + "%");
#endif
    }

    /// <summary>
    /// Get the video path from VideoData object and set the videoPlayer's video path
    /// </summary>
    /// <param name="videoPlayer"> The videoPlayer to set the video path to</param>
    /// <returns></returns>
    private bool SetVideoPath(ref VideoPlayer videoPlayer)
    {
        GameObject dataholder = GameObject.FindWithTag(this.dataTag);
        if (dataholder == null)
            return false;
        VideoData data = dataholder.GetComponent<VideoData>();
        if (data == null)
            return false;
        videoPlayer.url = data.VideoPath;
        Destroy(dataholder);
        return true;
    }

    /// <summary>
    /// This method parses the new image and compares it to the other images.
    /// After every comparision it will fire a progress event.
    /// </summary>
    /// <param name="source"> The VideoPlayer that contains the new image. </param>
    /// <param name="frameIndex"> The index of the videoframe that contains the image. </param>
    private async void ParseFrame(VideoPlayer source, long frameIndex)
    {
        Texture2D clonedTex = CopyTexture(source);
        //The first image is always unique
        if(this.textureColors.Count == 0)
            this.textureColors.Add(clonedTex, clonedTex.GetPixels32());
        else
            CompareTextures(clonedTex);
        OnImageProgress?.Invoke(this, frameIndex / (float)source.frameCount);
        //Check if there is the next frame is in the video's frame range
        if(frameIndex + (long) frameInterval > (long)source.frameCount)
        {
            source.frameReady -= ParseFrame;
            OnFinishedParsing?.Invoke(this, null);
            Destroy(this.gameObject);
        }
        else
        {
            source.frame = frameIndex + (long)frameInterval;
        }
    }

    /// <summary>
    /// Calculate the interval between the images to compare.
    /// </summary>
    /// <param name="source">The VideoPlayer containing the framerate</param>
    /// <returns></returns>
    private float CalculateFrameInterval(VideoPlayer source)
    {
        return (float)source.clip.frameRate * this.imageRecordInterval;
    }

    /// <summary>
    /// Copy the image from the videoPlayer to a Texture2D so its RGB values can be read.
    /// </summary>
    /// <param name="source"> The VideoPlayer containing the image/texture. </param>
    /// <returns></returns>
    private Texture2D CopyTexture(VideoPlayer source)
    {
        Texture2D clonedTexture = new Texture2D(source.texture.width, source.texture.height, TextureFormat.RGBA32, false);
        Graphics.Blit(source.texture, renderTexture);
        clonedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        clonedTexture.Apply();
        return clonedTexture;
    }

    /// <summary>
    /// Compare all images async to the new image.
    /// If the image is new, its added to the dictionary.
    /// </summary>
    /// <param name="texToCompare"> The texture/image to compare to the other textures. </param>
    private async void CompareTextures(Texture2D texToCompare)
    {
        var tasks = new List<Task<bool>>();
        Color32[] colorsToCompare = texToCompare.GetPixels32();

        foreach (Color32[] originalColor in this.textureColors.Values)
        {
            tasks.Add(CompareImageAsync(originalColor, colorsToCompare));
        }
        bool[] originals = await Task.WhenAll(tasks);
        foreach (bool value in originals)
        {
            if(value == false)
            {
                return;
            }
        }
        this.textureColors.Add(texToCompare, colorsToCompare);
    }

    /// <summary>
    /// Compares images based on the red value of the pixels.
    /// When the sum of the total difference is greater than the threashold,
    /// the image is considered unique.
    /// Returns true if the image is unique and false for a duplicate.
    /// </summary>
    /// <param name="originalColors"></param>
    /// <param name="colorsToCompare"></param>
    /// <returns></returns>
    private async Task<bool> CompareImageAsync(Color32[] originalColors, Color32[] colorsToCompare)
    {
        int differenceCounter = 0;
        for (int i = 0; i < originalColors.Length; i += pixelIncrement)
        {
            differenceCounter += Mathf.Abs(originalColors[i].r - colorsToCompare[i].r);
        }
        /*Threshold = duplicate percentage (minimumDifference) * pixels to compare 
        (originalColors.Length / pixelIncrement) * max difference per pixel (255); */
        float threshold =  minimumDifference * (originalColors.Length / pixelIncrement) * 255;
        return differenceCounter > threshold;
    }
}
