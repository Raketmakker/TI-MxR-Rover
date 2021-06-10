using Assets.Resources.Scripts.HLS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class HLSVideoPlayer : MonoBehaviour
{

    // Unity Attributes
    public VideoPlayer videoPlayer;
    
    public HLSType HLSStreamType;
    public string baseUrl;
    public string baseFilename;
    public string basePath;
    public string forcedSegmentExtension;
    public int videoResolutionIndex;
    public int sleepBetweenCommands;

    
    // Private Attributes
    private HLSStream stream;

    // Start is called before the first frame update
    void Start()
    {

        // Initialise the HLSStream with the same variables as prevously
        this.stream = new HLSStream();

        this.stream.HLSStreamType           = this.HLSStreamType;
        this.stream.baseUrl                 = this.baseUrl;
        this.stream.baseFilename            = this.baseFilename;
        this.stream.basePath                = this.basePath;
        this.stream.forcedSegmentExtension  = this.forcedSegmentExtension;
        this.stream.videoResolutionIndex    = this.videoResolutionIndex;
        this.stream.sleepBetweenCommands    = this.sleepBetweenCommands;
        this.stream.onSegmentReady         += this.onSegmentReady;

        this.stream.Start();        
    }

    private void OnDestroy()
    {

        this.stream.OnDestroy();
    }

    private void onSegmentReady(object sender, HLSInfo info)
    {

        Debug.Log("Callback on download in videoPlayer: " + info.path + "\\" + info.getCombinedForcedFilename());
        
        if (info.type == HLSType.Video)
        {

            this.videoPlayer.url = info.path + "\\" + info.getCombinedForcedFilename();
            this.videoPlayer.Play();
        }
    }
}
