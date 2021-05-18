using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

/**
 * @class: HLSStream
 * @description:
 *      The HLSStream class is created just to download the segments of a HLS stream.
 *      This had to be done in order to create a livestream in unity.
 *      
 *      It can:
 *          1: Download M3U8 files and decompose these.
 *          2: Use M3U8 files with seperate audio and video or combined audio and video.
 *          3: Download the newest whole segment depending on number and filesize.
 *          
 *      It can not:
 *          1: Use any other stream protocol.
 *          2: Use encription or authorisation.
 *      
 *      It is:
 *          1: Open to be updated when required.
 */
public class HLSStream : MonoBehaviour
{

    // UNITY FUNCTIONS
    void Start()        { this.start(); }
    void OnDestroy()    { this.stop(); }

    // PARAMETERS
    public string location;
    public string url;

    public string M3U8File;
    public int M3U8VideoIndex;

    public int sleepBetweenSegments;
    public bool isAudioAndVideo;

    private List<HLSSegmentStream> streams; 

    // BASE FUNCTIONS

    public HLSStream()
    {

        this.streams = new List<HLSSegmentStream>();
    }

    /**
     * @function: start()
     * @description: This function starts the first download. 
     * First it removes the old segment files.
     * Then the given callback will trigger the chain of async downloads.
     */
    public void start()
    {

        this.clearFolder(this.location + "\\audio\\");
        this.clearFolder(this.location + "\\M3U8s\\");
        this.clearFolder(this.location + "\\video\\");

        this.isRunning = true;

        if (!this.isAudioAndVideo)
            this.downloadM3U8FileAsync(this.M3U8File, new AsyncCompletedEventHandler(M3U8FileCallback));
        else
        {

            this.urls[this.videoKey] = this.url;
            this.M3U8Files[this.videoKey] = this.M3U8File;

            this.downloadM3U8FileAsync(this.M3U8File, new AsyncCompletedEventHandler(M3U8VideoCallback));
        }
    }

    /**
     * @funciton: stop()
     * @description: This function stops the HLSStream after the M3U8 setup.
     */
    public void stop()
    {

        this.isRunning = false;

        Debug.Log("\nHLSStream stopped");
    }

    /**
     * @function: clearFolder()
     * @param: string location
     * @description: This function removes all files in a folder.
     */
    private void clearFolder(string location)
    {

        DirectoryInfo folder = new DirectoryInfo(location);

        foreach (FileInfo file in folder.GetFiles())
            file.Delete();
    }

    // DOWNLOAD FUNCTIONS

    /**
     * @function: downloadM3U8FileAsync()
     * @param: string filename
     * @param: AsyncCompletedEventHandler callback
     * @description: This function will download a M3U8 file. This function is seperated to simplefy the code.
     */
    private void downloadM3U8FileAsync(string filename, AsyncCompletedEventHandler callback)
    {

        this.downloadFileAsync(this.url, this.location + "\\M3U8s\\", filename, callback);
    }

    /**
     * @function: downloadFileAsync()
     * @param: string url
     * @param: string location
     * @param: string filename
     * @param: AsyncCompeletedEventHandler callback
     * @description: This function downloads a single file asyncronous and sets a callback.
     */
    private void downloadFileAsync(string url, string location, string filename, AsyncCompletedEventHandler callback)
    {

        using (WebClient WC = new WebClient())
        {

            WC.DownloadFileCompleted += callback;
            WC.DownloadFileAsync(new Uri(url + filename), location + filename);
        }
    }

    /**
     * @function: downloadSegmentFilesAsync()
     * @param: string url
     * @param: string location
     * @param: ref int currentSegment
     * @param: AsyncCompletedEventHandler callback
     * @return: IEnumerable
     * @description: This function loops the downloadFileAsync function for each an every incoming segment.
     */
    private IEnumerable downloadSegmentFilesAsync(string url, string location, ref int currentSegment, AsyncCompletedEventHandler callback)
    {

        while (this.isRunning)
        {

            yield return new WaitForSeconds(this.sleepBetweenSegments);

            this.downloadFileAsync(url, location, this.segmentName + currentSegment + this.segmentType, callback);
        }

        
    }

    // FILE FUNCTIONS

    /**
     * @function: readFile()
     * @param: string location
     * @param: string filename
     * @param: string search = null
     * @param: bool isNegative = false
     * @return: List<string>
     * @description: This function returns the lines of a file, if the line contains the search string.
     */
    public List<string> readFile(string location, string filename, string search = null, bool isNegative = false)
    {

        System.IO.StreamReader file = new System.IO.StreamReader(location + filename);
        List<string> lines = new List<string>();
        string line;
        int counter = 0;

        while ((line = file.ReadLine()) != null)
        {

            if (search == null || isNegative && !line.Contains(search) || !isNegative && line.Contains(search))
                if (line.Length > 0)
                    lines.Add(line);

            counter++;
        }

        return lines;
    }

    /**
     * @function: readAudioOrVideoFile()
     * @param: out string url
     * @param: out int currentSegment
     * @param: string filename
     * @param: string seach = null
     * @param: bool isNegative = false
     * @description: This function sets variables depening on a read file. The usage of out makes it posible to use this function
     * for both the audio and video parameters. This way code won't be programmed twice and changes can be made on one location.
     * Steps:
     *  1: The lines are received by the search string and line 0 is Splitted on the '/'.
     *  2: The last wordt is used to receive the segmentName and segmentType.
     *  3: The url is received from line 0.
     *  4: The currentSegment is received from the last line.
     */
    private void readAudioOrVideoFile(out string url, out int currentSegment, string filename, string search = null, bool isNegative = false)
    {

        List<string> lines = this.readFile(this.location + "\\M3U8s\\", filename, search, isNegative);

        string[] segments = lines[lines.Count - 1].Split('/');
        this.setSegment(out currentSegment, segments[segments.Length - 1]);

        url = this.url;

        for (int i = 0; i < segments.Length - 1; i++)
            url += segments[i] + "/";
    }

    // SETTERS

    /**
     * @function: setSegment()
     * @param: out int currentSegment
     * @param: string segmentFilename
     * @description: This function slits the filename of a segment and sets the name and type.
     * This is prepared by changing all numbers to '#'.
     */
    private void setSegment(out int currentSegment, string segmentFilename)
    {

        string digit = "", name = "";

        foreach (char c in segmentFilename)
        {

            name  += (Char.IsDigit(c) ? '#' : c);
            digit += (Char.IsDigit(c) ? c : ' ');
        }

        string[] segmentParts = name.Split('#');

        this.segmentName = segmentParts[0];
        this.segmentType = segmentParts[segmentParts.Length - 1];

        currentSegment = Int32.Parse(digit);

        Debug.Log("segmentName & Type:\t" + this.segmentName + " & " + this.segmentType);
    }

    // M3U8 CALLBACKS

    /**
     * @function M3U8FileCallback()
     * @param: object sender
     * @param: AsyncCompletedEventArgs e
     * @description: This function deconstructs the initial M3U8 file.
     * Steps:
     *  1: The lines are received by searching on the original filename minus the extention.
     *  2: The M3U8AudioFile is received from line 0.
     *  3: The M3U8VideoFile is received from line [M3U8VideoIndex].
     *  4: Both the M3U8 audio and video file are downloaded asycronous.
     */
    private void M3U8FileCallback(object sender, AsyncCompletedEventArgs e)
    {

        Debug.Log("Downloaded:\t\t" + this.M3U8File);

        List<string> lines = this.readFile(this.location + "\\M3U8s\\", this.M3U8File, this.M3U8File.Split('.')[0], false);

        if (lines.Count > 0)
        {

            string[] audioLine = lines[0].Split('\"');

            for (int i = 0; i < audioLine.Length; i++)
                if (audioLine[i].Contains(this.M3U8File.Split('.')[0]))
                    this.M3U8Audio = audioLine[i];

            this.M3U8Video = lines[this.M3U8VideoIndex];

            Debug.Log("\nM3U8Audio:\t\t" + this.M3U8Audio);

            for (int i = 1; i < lines.Count; i++)
                Debug.Log("M3U8VideoIndex " + i + ":\t" + lines[i]);

            this.downloadM3U8FileAsync(this.M3U8Audio, new AsyncCompletedEventHandler(M3U8AudioCallback));
            this.downloadM3U8FileAsync(this.M3U8Video, new AsyncCompletedEventHandler(M3U8VideoCallback));
        }
        else
            this.start(); // restart
    }

    /**
     * @function: M3U8AudioCallback()
     * @param: object sender
     * @param: AsyncCompletedEventArgs e
     * @return: IEnumerator
     * @description: This funtion triggers both the readAudioOrVideoFile and downloadSegmentFilesAsync function with the audio parameters.
     */
    private IEnumerator downloadSegments(string filename)
    {

        Debug.Log("\nDownloaded:\t\t" + this.M3U8Audio);

        List<string> lines = this.readFile(this.location + "\\M3U8s\\", filename, "#", true);

        string[] segments  = lines[lines.Count - 1].Split('/');
        int currentSegment = this.setSegment(segments[segments.Length - 1]);

        string segmentUrl = this.url;

        for (int i = 0; i < segments.Length - 1; i++)
            segmentUrl += segments[i] + "/";

        Debug.Log("audioUrl:\t\t" + this.audioUrl + "\naudioSegment:\t\t" + this.audioSegment);

        while (this.isRunning)
        {

            string segmentFileName = this.segmentName + currentSegment + this.segmentType;

            this.downloadFileAsync(segmentUrl, this.location + "\\audio\\", segmentFileName, new AsyncCompletedEventHandler(audioSegmentCallback));

            yield return new WaitForSeconds(this.sleepBetweenSegments);
        }

        Debug.Log("DownloadSegmentFileAsync loop stopped");
    }

    /**
     * @function: M3U8VideoCallback()
     * @param: object sender
     * @param: AsyncCompletedEventArgs e
     * @description: This funtion triggers both the readAudioOrVideoFile and downloadSegmentFilesAsync function with the video parameters.
     */
    private void M3U8VideoCallback(object sender, AsyncCompletedEventArgs e)
    {

        Debug.Log("\nDownloaded:\t\t" + this.M3U8Video);

        this.readAudioOrVideoFile(out this.videoUrl, out this.videoSegment, this.M3U8Video, "#", true);

        Debug.Log("videoUrl:\t\t" + this.videoUrl + "\nvideoSegment:\t\t" + this.videoSegment);

        this.downloadSegmentFilesAsync(this.videoUrl, this.location + "\\video\\", ref this.videoSegment, new AsyncCompletedEventHandler(videoSegmentCallback));
    }

    // SEGMENT CALLBACKS

    /**
     * @function: audioSegmentCallback()
     * @function: videoSegmentCallback()
     * @param: object sender
     * @param: AsyncCompletedEventArgs e
     * @description: Both callbacks trigger the segmentCallback with their own parameters.
     */
    private void audioSegmentCallback(object sender, AsyncCompletedEventArgs e) { this.segmentCallback(ref this.audioSegment, ref this.audioSize, "audio"); }
    private void videoSegmentCallback(object sender, AsyncCompletedEventArgs e) { this.segmentCallback(ref this.videoSegment, ref this.videoSize, "video"); }

    /**
     * @function: segmentCallback()
     * @param: ref int currentSegment
     * @param: ref long fileSize
     * @param: string type
     * @description: This function handels the currentSegment depending on the state of the downloaded file
     * States:
     *  1: The file doesn't exist and a exception is thrown. Nothing happens.
     *  2: The file is longer then the original downloaded file. Download again until the size doesn't change.
     *  3: the file is just as long as the original downloaded file. The currentSegment is increased by 1.
     */
    private void segmentCallback(ref int currentSegment, ref long fileSize, string type)
    {

        try
        {

            long currentFileSize = new FileInfo(this.location + type + "\\" + this.segmentName + currentSegment + this.segmentType).Length;

            if (currentFileSize > fileSize)
                fileSize = currentFileSize;
            else if (currentFileSize == fileSize && fileSize != 0)
            {

                fileSize = 0;

                Debug.Log("Downloaded " + type + ":\t" + this.segmentName + currentSegment + this.segmentType + " - " + currentFileSize);

                currentSegment++;
            }
        }
        catch (FileNotFoundException e)
        {

            Debug.Log("Expection:\t\t" + e.Message);
        }
    }
}
