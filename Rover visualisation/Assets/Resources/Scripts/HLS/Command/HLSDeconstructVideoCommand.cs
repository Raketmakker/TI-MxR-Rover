using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{
    class HLSDeconstructVideoCommand : HLSCommand
    {

        // Constructor
        public HLSDeconstructVideoCommand(HLSInfo info) : base(info) { }

        // Overrides
        public override void execute()
        {

            HLSHelper helper   = HLSHelper.getInstance();
            HLSInfo videoInfo  = new HLSInfo(this.info);

            string search      = helper.removeExtension(this.info.filename);
                
            videoInfo.filename = helper.searchFileForLines(this.info.filename, this.info.path, search)[this.info.index + 1];
            videoInfo.type     = (videoInfo.type == HLSType.PreCombined) ? HLSType.Combined : HLSType.Video;

            HLSStream.addCommand(new HLSDownloadM3U8Command(videoInfo));
        }

        public override string getName() { return "HLSDeconstructVideoCommand"; }
    }
}
