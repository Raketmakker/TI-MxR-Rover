using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace HLS_Test_Module
{

    class HLSSegmentStream
    {

        // SEGMENT PARAMETERS
        private string segmentName;
        private string segmentType;
        private string segmentUrl;
        private int    segmentID;

        // FILE PARAMETERS
        public  string fileName;
        public  string fileUrl;
        public  string fileLocation;
        public  long   fileSize;

        // OTHER PARAMETERS
        public  string streamName;
        private bool   isRunning;
        private int    sleepBetweenSegments;

        // GENERAL FUNCTIONS
        public HLSSegmentStream(string streamName, string fileName, string fileUrl, string fileLocation, int sleepBetweenSegments = 1000, bool isStandard = false)
        {

            this.fileName = fileName;
            this.fileUrl = fileUrl;
            this.fileLocation = fileLocation + '\\' + streamName;

            this.streamName = streamName;
            this.sleepBetweenSegments = sleepBetweenSegments;

            if (isStandard)
                this.standardStart();
        }

        public void standardStart()
        {

            this.downloadFile(new AsyncCompletedEventHandler(this.standardSegmentSetupCallback));
        }

        public void stop()
        {

            this.isRunning = false;
        }

        // DOWNLOAD FILE ASYNC FUNCTIONS
        public void downloadFile(AsyncCompletedEventHandler callback, string url = null, string name = null, string location = null)
        {

            if (url == null)        url = this.fileUrl;
            if (name == null)       name = this.fileName;
            if (location == null)   location = this.fileLocation;

            if (!Directory.Exists(location))
                 Directory.CreateDirectory(location);

            using (WebClient WC = new WebClient())
            {

                WC.DownloadFileCompleted += callback;
                WC.DownloadFileAsync(new Uri(url + '/' + name), location + '\\' + name);
            }
        }

        public void downloadSegmentsLoop(AsyncCompletedEventHandler callback, string url = null, string name = null, string location = null)
        {

            this.isRunning = true;

            while (this.isRunning)
            {

                this.downloadFile(callback, url, name, location);

                Thread.Sleep(this.sleepBetweenSegments);
            }

            Console.WriteLine(this.streamName + "'s download segments loop stopped");
        }

        // READ FUNCTIONS
        public List<string> readFile(string search = null, bool isNegative = false, string name = null, string location = null)
        {

            if (name == null)       name = this.fileName;
            if (location == null)   location = this.fileLocation;

            StreamReader fileReader = new StreamReader(location + '\\' + name);
            
            List<string> lines = new List<string>();
            string line;
            
            while ((line = fileReader.ReadLine()) != null)
                if (line.Length > 0 && (search == null || isNegative && !line.Contains(search) || !isNegative && line.Contains(search)))
                    lines.Add(line);

            return lines;
        }

        // SEGMENT FUNCTIONS
        private void processSegmentSetupFile(string search = null, bool isNegative = false, string name = null, string location = null)
        {

            List<string> lines = this.readFile(search, isNegative, name, location);

            string[] lineSplits = lines[lines.Count - 1].Split('/');
            this.processSegmentParameters(lineSplits[lineSplits.Length - 1]);

            for (int i = 0; i < lineSplits.Length - 1; i++)
                this.segmentUrl += ('/' + lineSplits[i]);
        }

        private void processSegmentParameters(string fullSegmentName)
        {

            string digit = "", name = "";

            foreach (char c in fullSegmentName)
            {

                name += (Char.IsDigit(c) ? '#' : c);
                digit += (Char.IsDigit(c) ? c : ' ');
            }

            string[] segments = name.Split('#');

            this.segmentName = segments[0];
            this.segmentID   = Int32.Parse(digit);
            this.segmentType = segments[segments.Length - 1];
        }

        private string fullSegmentName() { return (this.segmentName + this.segmentID + this.segmentType); }

        // BASIC CALLBACKS
        public void standardSegmentSetupCallback(object sender, AsyncCompletedEventArgs args)
        {

            Console.WriteLine('\n' + this.streamName + " downloaded:\t" + this.fileName);

            this.processSegmentSetupFile("#", true);

            Console.WriteLine(this.streamName + "'s segmentUrl:\t" + this.segmentUrl);
            Console.WriteLine(this.streamName + "'s fullSegmentName:\t" + this.fullSegmentName());

            this.downloadSegmentsLoop(
                new AsyncCompletedEventHandler(this.standardSegmentCallback),
                this.fileUrl + this.segmentUrl,
                this.fullSegmentName(),
                this.fileLocation
            );
        }
        
        public void standardSegmentCallback(object sender, AsyncCompletedEventArgs args)
        {

            try
            {

                string location = this.fileLocation + '\\' + this.fullSegmentName();

                long size = new FileInfo(location).Length;

                if (size > this.fileSize)
                    this.fileSize = size;

                else if (this.fileSize != 0 && size == this.fileSize)
                {

                    Console.WriteLine(this.streamName + " downloaded:\t" + this.fullSegmentName() + " with size: " + this.fileSize);

                    this.fileSize = 0;
                    this.segmentID++;
                }
            }
            catch (FileNotFoundException excpetion)
            {

                Console.WriteLine(this.streamName + "'s expection:\t" + excpetion.Message);
            }
        }
    }
}
