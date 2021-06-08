using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{
    class HLSDeconstructAudioCommand : HLSCommand
    {

        // Constructor
        public HLSDeconstructAudioCommand(HLSInfo info) : base(info) { }

        // Overrides
        public override void execute()
        {

            HLSHelper helper    = HLSHelper.getInstance();
            HLSInfo   audioInfo = new HLSInfo(this.info);

            string search       = helper.removeExtension(this.info.filename);
            string line         = helper.searchFileForLines(this.info.filename, this.info.path, search)[0];

            string[] lineParts  = line.Split('"');

            foreach (string part in lineParts)
                if (part.Contains(search))
                    audioInfo.filename = part;

            audioInfo.type = HLSType.Audio;

            HLSStream.addCommand(new HLSDownloadM3U8Command(audioInfo));
        }

        public override string getName() { return "HLSDeconstructAudioCommand"; }
    }
}
