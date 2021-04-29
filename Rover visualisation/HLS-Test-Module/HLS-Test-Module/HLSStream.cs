using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace HLS_Test_Module
{

    class HLSStream
    {

        public string url;
        public string location;

        public string m3u8File;
        public string segmentName;
        public string segmentType;

        public int m3u8VideoIndex;

        private string m3u8VideoFile;
        private string m3u8AudioFile;

        private int currentVideoSegment;
        private int currentAudioSegment;

        // constructor
        public HLSStream()
        {

            this.m3u8VideoIndex = 0x00;

            this.currentVideoSegment = 0x00;
            this.currentAudioSegment = 0x00;
        }

        // starts the first download in the chain of asyncCallbacks
        public void start()
        {

            this.downloadFileAsync(this.m3u8File + ".m3u8", new AsyncCompletedEventHandler(m3u8Callback));
        }

        // download Async file function
        public void downloadFileAsync(string filename, AsyncCompletedEventHandler callback)
        {

            using(WebClient WC = new WebClient())
            {

                WC.DownloadFileCompleted += callback;
                WC.DownloadFileAsync(new System.Uri(this.url + filename), this.location + filename);
            }
        }

        // read the file for valueble lines
        public List<string> readFile(string location, string filename, string search)
        {

            System.IO.StreamReader file = new System.IO.StreamReader(location + filename);
            List<string> lines = new List<string>(); 
            string line;
            int counter = 0;

            while ((line = file.ReadLine()) != null)
            {

                if (line.Contains(search))
                    lines.Add(line);

                counter++;
            }

            return lines;
        }

        // m3u8 callbacks
        private void m3u8Callback(object sender, AsyncCompletedEventArgs e)
        {

            List<string> lines = this.readFile(this.location, this.m3u8File + ".m3u8", this.m3u8File);

            this.m3u8AudioFile = lines[0].Split("URI=\"")[1];
            this.m3u8AudioFile = this.m3u8AudioFile.Substring(0, this.m3u8AudioFile.Length - 1);

            this.m3u8VideoFile = lines[this.m3u8VideoIndex + 1];

            this.downloadFileAsync(this.m3u8AudioFile, new AsyncCompletedEventHandler(m3u8AudioCallback));
            this.downloadFileAsync(this.m3u8VideoFile, new AsyncCompletedEventHandler(m3u8VideoCallback));
        }

        private void m3u8VideoCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("Downloaded the m3u8 video file");
        }

        private void m3u8AudioCallback(object sender, AsyncCompletedEventArgs e)
        {

            Console.WriteLine("Downloaded the m3u8 audio file");
        }
    }
}
