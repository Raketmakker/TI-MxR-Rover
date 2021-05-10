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

        // public parameters
        public string location;
        public string url;

        public string m3u8File;

        public int m3u8VideoIndex;

        public bool isRunning;

        // private parameters
        private string m3u8VideoFile;
        private string m3u8AudioFile;

        private string videoUrl;
        private string audioUrl;

        private int currentVideoSegment;
        private int currentAudioSegment;

        private string segmentName;
        private string segmentType;

        // constructor
        public HLSStream()
        {

            this.m3u8VideoIndex = 0x01;

            this.currentVideoSegment = 0x00;
            this.currentAudioSegment = 0x00;

            this.isRunning = true;
        }

        // starts the first download in the chain of asyncCallbacks
        public void start()
        {

            this.downloadM3u8FileAsync(this.m3u8File, new AsyncCompletedEventHandler(m3u8Callback));
        }

        // download Async file functions
        private void downloadM3u8FileAsync(string filename, AsyncCompletedEventHandler callback)
        {

            this.downloadFileAsync(this.url, this.location + "m3u8s\\", filename, callback);
        }
        private void downloadFileAsync(string url, string location, string filename, AsyncCompletedEventHandler callback)
        {

            using (WebClient WC = new WebClient())
            {

                WC.DownloadFileCompleted += callback;
                WC.DownloadFileAsync(new Uri(url + filename), location + filename);
            }
        }
        private void downloadSegmentFilesAsync(string url, string location, ref int currentSegment, AsyncCompletedEventHandler callback)
        {

            while (this.isRunning)
            {

                this.downloadFileAsync(url, location, this.segmentName + currentSegment + this.segmentType, callback);

                Thread.Sleep(1);
            }
        }

        // read the file for valueble lines
        public List<string> readFile(string location, string filename, string search = null)
        {

            System.IO.StreamReader file = new System.IO.StreamReader(location + filename);
            List<string> lines = new List<string>();
            string line;
            int counter = 0;

            while ((line = file.ReadLine()) != null)
            {

                if (search == null || line.Contains(search))
                    lines.Add(line);

                counter++;
            }

            return lines;
        }

        private void readAudioOrVideoFile(out string url, out int currentSegment, string filename, string search = null)
        {

            List<string> lines = this.readFile(this.location + "m3u8s\\", filename, search);

            string[] segments = lines[0].Split('/');

            this.setSegment(segments[segments.Length - 1]);

            url = this.url + lines[0].Split(this.segmentName)[0];
            currentSegment = Int32.Parse(lines[lines.Count - 1].Split(this.segmentName)[1].Split(this.segmentType)[0]);
        }

        // Setters
        private void setSegment(string filename)
        {

            if (this.segmentName == null || this.segmentType == null)
            {

                this.segmentName = filename.Split('0')[0];
                this.segmentType = filename.Split('0')[1];

                Console.WriteLine("segmentName:\t\t" + this.segmentName + "\nsegmentType:\t\t" + this.segmentType);
            }
        }

        // m3u8 callbacks
        private void m3u8Callback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("Downloaded:\t\t" + this.m3u8File);

            List<string> lines = this.readFile(this.location + "m3u8s\\", this.m3u8File, this.m3u8File.Split('.')[0]);

            this.m3u8AudioFile = lines[0].Split("URI=\"")[1].Split('"')[0];
            this.m3u8VideoFile = lines[this.m3u8VideoIndex];

            Console.WriteLine("\nm3u8AudioFile:\t\t" + this.m3u8AudioFile);

            for (int i = 1; i < lines.Count; i++)
                Console.WriteLine("m3u8VideoIndex " + i + ":\t" + lines[i]);

            this.downloadM3u8FileAsync(this.m3u8AudioFile, new AsyncCompletedEventHandler(m3u8AudioCallback));
            this.downloadM3u8FileAsync(this.m3u8VideoFile, new AsyncCompletedEventHandler(m3u8VideoCallback));
        }

        private void m3u8AudioCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("\nDownloaded:\t\t" + this.m3u8AudioFile);

            this.readAudioOrVideoFile(out this.audioUrl, out this.currentAudioSegment, this.m3u8AudioFile, "audio");

            Console.WriteLine("audioUrl:\t\t" + this.audioUrl + "\ncurrentAudioSegment:\t" + this.currentAudioSegment);

            this.currentAudioSegment -= 10;

            this.downloadSegmentFilesAsync(this.audioUrl, this.location + "audio\\", ref this.currentAudioSegment, new AsyncCompletedEventHandler(audioSegmentCallback));
        }

        private void m3u8VideoCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("\nDownloaded:\t\t" + this.m3u8VideoFile);

            this.readAudioOrVideoFile(out this.videoUrl, out this.currentVideoSegment, this.m3u8VideoFile, "video");

            Console.WriteLine("videoUrl:\t\t" + this.videoUrl + "\ncurrentVideoSegment:\t" + this.currentVideoSegment);

            this.currentVideoSegment -= 10;

            this.downloadSegmentFilesAsync(this.videoUrl, this.location + "video\\", ref this.currentVideoSegment, new AsyncCompletedEventHandler(videoSegmentCallback));
        }

        private void audioSegmentCallback(object sender, AsyncCompletedEventArgs e)
        {

            if (new FileInfo(this.location + "audio\\" + this.segmentName + this.currentAudioSegment + this.segmentType).Length > 0)
            {

                Console.WriteLine("Downloaded audio:\t" + this.segmentName + this.currentAudioSegment + this.segmentType);
                this.currentAudioSegment++;
            }
        }

        private void videoSegmentCallback(object sender, AsyncCompletedEventArgs e)
        {

            if (new FileInfo(this.location + "video\\" + this.segmentName + this.currentVideoSegment + this.segmentType).Length > 0)
            {

                Console.WriteLine("Downloaded video:\t" + this.segmentName + this.currentVideoSegment + this.segmentType);

                this.currentVideoSegment++;
            }
        }
    }
}
