using System;
using System.IO;
using System.Threading;

namespace HLS_Test_Module
{
    class Program
    {

        static void Main(string[] args)
        {

            HLSStream HLS = new HLSStream();

            HLS.baseUrl              = "https://bitmovin-a.akamaihd.net/content/MI201109210084_1/m3u8s";
            HLS.baseLocation         = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName + "\\HLSFiles";
            HLS.firstFileName        = "f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8";
            HLS.videoResolutionIndex = 5;
            HLS.sleepBetweenSegments = 250;
            HLS.isAudioAndVideo      = false;
            HLS.isForcedMP4          = true;

            HLS.start();

            Console.Read();

            HLS.stop();

            Thread.Sleep(5000);
        }
    }
}
