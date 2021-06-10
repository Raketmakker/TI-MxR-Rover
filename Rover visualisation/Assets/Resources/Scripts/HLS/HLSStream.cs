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

public class HLSStream : MonoBehaviour
{

    // Unity Attributes
    public EventHandler<HLSInfo> onSegmentReady;
    public HLSType               HLSStreamType;
    public string                baseUrl;
    public string                baseFilename;
    public string                basePath;
    public string                forcedSegmentExtension;
    public int                   videoResolutionIndex;
    public int                   sleepBetweenCommands;

    /**
        * @functions: Unity functions
        * @description: These functions are used by unity to startup the HLS stream.
        */
    public void Start()     { this.start(); }
    public void OnDestroy() { this.stop();  }

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

        if (this.basePath == "")
            this.basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        
        HLSHelper.getInstance().clearPath(this.basePath);

        this.basePath += "\\HLSDownloads"; // Added here for preventive measure in .clearPath().

        // turns the unity paramters to a Info class
        HLSInfo info         = new HLSInfo();
        info.type            = this.HLSStreamType;
        info.url             = this.baseUrl;
        info.filename        = this.baseFilename;
        info.path            = this.basePath;
        info.extensionForced = this.forcedSegmentExtension;
        info.index           = this.videoResolutionIndex;
        info.onSegmentReady  = this.onSegmentReady;

        addCommand(new HLSDownloadM3U8Command(info));

        this.HLSCommandThread = new Thread(this.commandLoop);
        this.HLSCommandThread.Start();
    }

    private void stop()
    {

        this.isRunning = false;

        try                             { this.HLSCommandThread.Abort();                } 
        catch (ThreadAbortException e)  { Debug.Log("HLSStream Stopped: " + e.Message); }

        commands = null;
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

