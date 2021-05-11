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

            HLS.url             = "http://192.168.68.185:8080/hls/";
            HLS.location        = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName + "\\HLSFiles\\";
            HLS.m3u8File        = "test.m3u8";
            HLS.m3u8VideoIndex  = 5;
            HLS.isAudioAndVideo = true;

            HLS.start();

            Console.Read();

            HLS.stop();

            Thread.Sleep(5000);
        }
    }
}
