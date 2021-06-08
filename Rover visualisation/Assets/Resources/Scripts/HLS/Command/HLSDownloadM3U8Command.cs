using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resources.Scripts.HLS.Command
{

    class HLSDownloadM3U8Command : HLSCommand
    {

        // Constructor
        public HLSDownloadM3U8Command(HLSInfo info) : base(info) { }

        // Overrides
        public override void execute() 
        {

            HLSHelper.getInstance().downloadFileAsync(
                new AsyncCompletedEventHandler(this.callback),
                this.info.url,
                this.info.filename,
                this.info.path
            );
        }

        public override string getName() { return "HLSDownloadM3U8Command"; }

        // Callback
        public void callback(object sender, AsyncCompletedEventArgs args)
        {

            if (HLSHelper.getInstance().isDownloaded(this.info.filename, this.info.path))
                HLSStream.addCommand(new HLSProcessM3U8Command(this.info));

            else
                HLSStream.addCommand(this);
        }
    }
}
