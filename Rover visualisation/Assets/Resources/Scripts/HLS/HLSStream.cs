using Assets.Resources.Scripts.HLS;
using Assets.Resources.Scripts.HLS.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

class HLSStream : MonoBehaviour
{

    // Unity Attributes
    public HLSType  HLSStreamType;
    public string   baseUrl;
    public string   baseFilename;
    public string   basePath;
    public string   forcedSegmentExtension;
    public int      videoResolutionIndex;
    public int      sleepBetweenCommands;

    /**
        * @functions: Unity functions
        * @description: These functions are used by unity to startup the HLS stream.
        */
    void Start()     { this.start(); }
    void OnDestroy() { this.stop();  }

    // Private attributes
    private Thread  HLSCommandThread;
    private bool    isRunning;

    // Static functions
    public  static void addCommand(HLSCommand command) { commands.Add(command); }
    private static List<HLSCommand> commands;

    // Start / Stop
    private void start()
    {

        commands = new List<HLSCommand>();

        // turns the unity paramters to a Info class
        HLSInfo info         = new HLSInfo();
        info.type            = this.HLSStreamType;
        info.url             = this.baseUrl;
        info.filename        = this.baseFilename;
        info.path            = this.basePath;
        info.extensionForced = this.forcedSegmentExtension;
        info.index           = this.videoResolutionIndex;

        if (info.path == "")
            info.path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        HLSHelper.getInstance().clearPath(info.path);

        info.path += "\\HLSDownloads"; // Added here for preventive measure in .clearPath().

        addCommand(new HLSDownloadM3U8Command(info));

        this.HLSCommandThread = new Thread(this.commandLoop);
        this.HLSCommandThread.Start();
    }

    private void stop()
    {

        this.isRunning = false;

        try                             { this.HLSCommandThread.Abort();                } 
        catch (ThreadAbortException e)  { Debug.Log("HLSStream Stopped: " + e.Message); }
    }

    // Loop
    private void commandLoop()
    {

        this.isRunning = true;

        while (this.isRunning)
        {
            
            Thread.Sleep(this.sleepBetweenCommands);

            if (commands.Count() > 0)
            {

                HLSCommand command = commands[0];

                command.execute();
                commands.Remove(command);

                Debug.Log("Executed " + command.getName());
            }
        }
    }
}

