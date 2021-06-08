using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{
    class HLSProcessM3U8Command : HLSCommand
    {

        // Constructor
        public HLSProcessM3U8Command(HLSInfo info) : base(info) { }

        // Overrides
        public override void execute()
        {

            HLSType type = this.info.type;

            if (type == HLSType.Audio || type == HLSType.Video || type == HLSType.Combined)
                HLSStream.addCommand(new HLSDeconstructSegmentCommand(this.info));

            else
            {

                if (type == HLSType.PreAudio || type == HLSType.PreSeperate)
                    HLSStream.addCommand(new HLSDeconstructAudioCommand(this.info));

                if (type == HLSType.PreVideo || type == HLSType.PreCombined || type == HLSType.PreSeperate)
                    HLSStream.addCommand(new HLSDeconstructVideoCommand(this.info));
            }
        }

        public override string getName() { return "HLSProcessM3U8Command"; }
    }
}
