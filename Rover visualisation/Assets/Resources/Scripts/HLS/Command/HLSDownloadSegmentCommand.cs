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

            string downloadFilename = null;
            
            if (this.info.extensionForced != null) 
                downloadFilename = helper.replaceExtension(this.info.getCombinedFilename(), this.info.extensionForced);

            helper.downloadFileAsync(
                new AsyncCompletedEventHandler(this.callback),
                this.info.url,
                this.info.getCombinedFilename(),
                this.info.path,
                downloadFilename
            );
        }

        public override string getName() { return "HLSDownloadSegmentCommand"; }

        // Callback
        public void callback(object sender, AsyncCompletedEventArgs args)
        {

            HLSHelper helper = HLSHelper.getInstance();

            long size = helper.getFileSize(this.info.filename, this.info.path);

            if (helper.isDownloaded(this.info.filename, this.info.path, size, this.fileSize))
            {

                this.info.index += 1;
                this.fileSize = 0;
            }
            else
                this.fileSize = size;
                
            HLSStream.addCommand(this);
        }
    }
}
