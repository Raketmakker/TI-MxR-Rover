using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{
    class HLSDownloadSegmentCommand : HLSCommand
    {

        // Attributes
        long fileSize;

        // Constructor
        public HLSDownloadSegmentCommand(HLSInfo info) : base(info) { this.fileSize = 0; }

        // Overrides
        public override void execute()
        {

            HLSHelper helper = HLSHelper.getInstance();

            helper.downloadFileAsync(
                new AsyncCompletedEventHandler(this.callback),
                this.info.url,
                this.info.getCombinedFilename(),
                this.info.path,
                this.info.getCombinedForcedFilename()
            );
        }

        public override string getName() { return "HLSDownloadSegmentCommand"; }

        // Callback
        public void callback(object sender, AsyncCompletedEventArgs args)
        {

            HLSHelper helper = HLSHelper.getInstance();

            long size = helper.getFileSize(this.info.getCombinedForcedFilename(), this.info.path);

            if (helper.isDownloaded(this.info.filename, this.info.path, size, this.fileSize))
            {
                
                this.info.onSegmentReady?.Invoke(null, this.info);

                this.fileSize = 0;
                this.info.index += 1;
            }
            else
                this.fileSize = size;
                
            HLSStream.addCommand(this);
        }
    }
}
