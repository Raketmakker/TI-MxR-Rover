using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace HLS_Test_Module
{

    class HLSStream
    {

        // PUBLIC PARAMETERS
        public string location;
        public string url;

        public string m3u8File;

        public int m3u8VideoIndex;

        public bool isRunning;
        public bool isVideoOnly;

        // PRIVATE PARAMETERS
        private string m3u8VideoFile;
        private string m3u8AudioFile;

        private string videoUrl;
        private string audioUrl;

        private int currentVideoSegment;
        private int currentAudioSegment;

        private string segmentName;
        private string segmentType;

        // BASE FUNCTIONS

        /**
         * @function: constructor
         * @description: This is the constructor of the class. It sets the first few variables.
         */
        public HLSStream()
        {

            this.m3u8VideoIndex = 0x01;

            this.isRunning = true;
        }

        /**
         * @function: start()
         * @description: This function starts the first download. 
         * First it removes the old segment files.
         * Then the given callback will trigger the chain of async downloads.
         */
        public void start()
        {

            this.clearFolder(this.location + "audio\\");
            this.clearFolder(this.location + "m3u8s\\");
            this.clearFolder(this.location + "video\\");

            if (!this.isVideoOnly)
                this.downloadM3u8FileAsync(this.m3u8File, new AsyncCompletedEventHandler(m3u8Callback));
            else
            {

                this.videoUrl = this.url;
                this.m3u8VideoFile = this.m3u8File;

                this.downloadM3u8FileAsync(this.m3u8VideoFile, new AsyncCompletedEventHandler(m3u8VideoCallback));
            }
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
         * @function: downloadM3u8FileAsync()
         * @param: string filename
         * @param: AsyncCompletedEventHandler callback
         * @description: This function will download a m3u8 file. This function is seperated to simplefy the code.
         */
        private void downloadM3u8FileAsync(string filename, AsyncCompletedEventHandler callback)
        {

            this.downloadFileAsync(this.url, this.location + "m3u8s\\", filename, callback);
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
         * @description: This function loops the downloadFileAsync function for each an every incoming segment.
         */
        private void downloadSegmentFilesAsync(string url, string location, ref int currentSegment, AsyncCompletedEventHandler callback)
        {

            while (this.isRunning)
            {

                this.downloadFileAsync(url, location, this.segmentName + currentSegment + this.segmentType, callback);

                Thread.Sleep(100);
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

            List<string> lines = this.readFile(this.location + "m3u8s\\", filename, search, isNegative);

            string[] segments = lines[0].Split('/');

            this.setSegment(segments[segments.Length - 1]);

            url = this.url + lines[0].Split(this.segmentName)[0];
            currentSegment = Int32.Parse(lines[lines.Count - 1].Split(this.segmentName)[1].Split(this.segmentType)[0]);
        }

        // SETTERS

        /**
         * @function: setSegment()
         * @param: string segmentFilename
         * @description: This function slits the filename of a segment and sets the name and type.
         * This is prepared by changing all numbers to '#'.
         */
        private void setSegment(string segmentFilename)
        {

            if (this.segmentName == null || this.segmentType == null)
            {

                string name = "";

                foreach (char c in segmentFilename)
                    name += (Char.IsDigit(c) ? '#' : c);

                string[] segments = name.Split('#');

                this.segmentName = segments[0];
                this.segmentType = segments[segments.Length - 1];

                Console.WriteLine("segmentName:\t\t" + this.segmentName + "\nsegmentType:\t\t" + this.segmentType);
            }
        }

        // M3U8 CALLBACKS

        /**
         * @function m3u8Callback()
         * @param: object sender
         * @param: AsyncCompletedEventArgs e
         * @description: This function deconstructs the initial m3u8 file.
         * Steps:
         *  1: The lines are received by searching on the original filename minus the extention.
         *  2: The m3u8AudioFile is received from line 0.
         *  3: The m3u8VideoFile is received from line [m3u8VideoIndex].
         *  4: Both the m3u8 audio and video file are downloaded asycronous.
         */
        private void m3u8Callback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("Downloaded:\t\t" + this.m3u8File);

            List<string> lines = this.readFile(this.location + "m3u8s\\", this.m3u8File, this.m3u8File.Split('.')[0], false);

            this.m3u8AudioFile = lines[0].Split("URI=\"")[1].Split('"')[0];
            this.m3u8VideoFile = lines[this.m3u8VideoIndex];

            Console.WriteLine("\nm3u8AudioFile:\t\t" + this.m3u8AudioFile);

            for (int i = 1; i < lines.Count; i++)
                Console.WriteLine("m3u8VideoIndex " + i + ":\t" + lines[i]);

            this.downloadM3u8FileAsync(this.m3u8AudioFile, new AsyncCompletedEventHandler(m3u8AudioCallback));
            this.downloadM3u8FileAsync(this.m3u8VideoFile, new AsyncCompletedEventHandler(m3u8VideoCallback));
        }

        /**
         * @function: m3u8AudioCallback()
         * @param: object sender
         * @param: AsyncCompletedEventArgs e
         * @description: This funtion triggers both the readAudioOrVideoFile and downloadSegmentFilesAsync function with the audio parameters.
         */
        private void m3u8AudioCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("\nDownloaded:\t\t" + this.m3u8AudioFile);

            this.readAudioOrVideoFile(out this.audioUrl, out this.currentAudioSegment, this.m3u8AudioFile, "#", true);

            Console.WriteLine("audioUrl:\t\t" + this.audioUrl + "\ncurrentAudioSegment:\t" + this.currentAudioSegment);

            this.downloadSegmentFilesAsync(this.audioUrl, this.location + "audio\\", ref this.currentAudioSegment, new AsyncCompletedEventHandler(audioSegmentCallback));
        }

        /**
         * @function: m3u8VideoCallback()
         * @param: object sender
         * @param: AsyncCompletedEventArgs e
         * @description: This funtion triggers both the readAudioOrVideoFile and downloadSegmentFilesAsync function with the video parameters.
         */
        private void m3u8VideoCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("\nDownloaded:\t\t" + this.m3u8VideoFile);

            this.readAudioOrVideoFile(out this.videoUrl, out this.currentVideoSegment, this.m3u8VideoFile, "#", true);

            Console.WriteLine("videoUrl:\t\t" + this.videoUrl + "\ncurrentVideoSegment:\t" + this.currentVideoSegment);

            this.downloadSegmentFilesAsync(this.videoUrl, this.location + "video\\", ref this.currentVideoSegment, new AsyncCompletedEventHandler(videoSegmentCallback));
        }

        // SEGMENT CALLBACKS

        /**
         * @function: audioSegmentCallback()
         * @function: videoSegmentCallback()
         * @param: object sender
         * @param: AsyncCompletedEventArgs e
         * @description: Both callbacks trigger the segmentCallback with their own parameters.
         */
        private void audioSegmentCallback(object sender, AsyncCompletedEventArgs e) { this.segmentCallback(ref this.currentAudioSegment, "audio"); }
        private void videoSegmentCallback(object sender, AsyncCompletedEventArgs e) { this.segmentCallback(ref this.currentVideoSegment, "video"); }

        /**
         * @function: segmentCallback()
         * @param: ref int currentSegment
         * @param: string type
         * @description: This function handels the currentSegment depending on the state of the downloaded file
         * States:
         *  1: The file doesn't exist and a exception is thrown. Nothing happens.
         *  2: The file is empty (The FileInfo length is 0). Nothing happens.
         *  3: the file isn't empty. The currentSegment is increased by 1.
         */
        private void segmentCallback(ref int currentSegment, string type)
        {

            try
            {

                if (new FileInfo(this.location + type + "\\" + this.segmentName + currentSegment + this.segmentType).Length > 0)
                {

                    Console.WriteLine("Downloaded " + type + ":\t" + this.segmentName + currentSegment + this.segmentType);

                    currentSegment++;
                }
            }
            catch (FileNotFoundException e) 
            {

                Console.WriteLine("Expection:\t\t" + e.Message);
            }
        }
    }
}
