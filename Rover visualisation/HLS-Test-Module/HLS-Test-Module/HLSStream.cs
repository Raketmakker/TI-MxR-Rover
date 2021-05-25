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
        public  string baseLocation;
        public  string baseUrl;

        public  string firstFileName;
        public  int    videoResolutionIndex;
        public  int    sleepBetweenSegments;

        public  bool   isAudioAndVideo;
        public  bool   isForcedMP4;

        // HLS SEGMENT STREAMS
        private List<HLSSegmentStream> streams;

        // BASE FUNCTIONS
        public void start()
        {

            string streamName = (this.isAudioAndVideo) ? "AudioAndVideo" : "M3U8";

            this.clearDirectory();

            this.streams = new List<HLSSegmentStream>();
            this.streams.Add(new HLSSegmentStream(streamName, this.firstFileName, this.baseUrl, this.baseLocation, this.sleepBetweenSegments, this.isForcedMP4));

            if (this.isAudioAndVideo)
                this.streams[0].standardStart();
            else
                this.streams[0].downloadFile(new AsyncCompletedEventHandler(this.M3U8Callback));
        }

        public void stop()
        {

            Console.WriteLine("\nHLSStream stopped");

            foreach (HLSSegmentStream stream in this.streams)
            {

                stream.stop();

                Console.WriteLine(stream.streamName + " HLSSegmentStream stopped");
            }
        }

        private void clearDirectory()
        {

            DirectoryInfo mainDirectory = new DirectoryInfo(this.baseLocation);

            foreach (DirectoryInfo directory in mainDirectory.GetDirectories())
            {

                foreach (FileInfo file in directory.GetFiles())
                    file.Delete();
                
                directory.Delete();
            }
        }

        // CALLBACKS
        private void M3U8Callback(object sender, AsyncCompletedEventArgs e)
        {

            HLSSegmentStream stream = this.streams[0];

            Console.WriteLine(stream.streamName + " downloaded:\t" + stream.fileName);

            List<string> lines = stream.readFile(stream.fileName.Split('.')[0]);

            if (lines.Count > 0)
                this.processStreamBuildFile(stream, lines);
            else
                this.start();
        }

        private void processStreamBuildFile(HLSSegmentStream stream, List<string> lines)
        {

            string audioFile = "", videoFile;
            string[] audioLine = lines[0].Split('\"');

            for (int i = 0; i < audioLine.Length; i++)
                if (audioLine[i].Contains(stream.fileName.Split('.')[0]))
                    audioFile = audioLine[i];

            videoFile = lines[this.videoResolutionIndex];

            Console.WriteLine("\naudioFile:\t\t" + audioFile);
            Console.WriteLine("videoFile:\t\t" + videoFile + "\n");

            for (int i = 1; i < lines.Count; i++)
                Console.WriteLine("videoResolutionIndex " + i + ":\t" + lines[i]);

            this.streams.Add(new HLSSegmentStream("Audio", audioFile, this.baseUrl, this.baseLocation, this.sleepBetweenSegments, this.isForcedMP4, true));
            this.streams.Add(new HLSSegmentStream("Video", videoFile, this.baseUrl, this.baseLocation, this.sleepBetweenSegments, this.isForcedMP4, true));
        }
    }
}
