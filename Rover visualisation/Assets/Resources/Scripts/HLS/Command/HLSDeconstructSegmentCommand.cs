using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{
    class HLSDeconstructSegmentCommand : HLSCommand
    {

        // Constructor
        public HLSDeconstructSegmentCommand(HLSInfo info) : base(info) { }

        // Overrides
        public override void execute()
        {

            HLSHelper helper        = HLSHelper.getInstance();
            HLSInfo segmentInfo     = new HLSInfo(this.info);

            List<string> lines      = helper.searchFileForLines(this.info.filename, this.info.path, "#", false);
            string[] lineParts      = lines[lines.Count - 1].Split('/');
            string[] nameParts      = lineParts[lineParts.Length - 1].Split('.');

            string digit = "";

            for (int i = nameParts[0].Length - 1; i >= 0; i--)
            {

                if (Char.IsDigit(nameParts[0][i]))
                    digit = (nameParts[0][i] + digit);
                else
                {

                    nameParts[0] = nameParts[0].Substring(0, i + 1);
                    break;
                }
            }

            segmentInfo.filename    =       nameParts[0];
            segmentInfo.extension   = '.' + nameParts[nameParts.Length - 1];
            segmentInfo.index       =       Int32.Parse(digit);

            for (int i = 0; i < lineParts.Length - 1; i++)
                segmentInfo.url += ('/' + lineParts[i]);

            switch (this.info.type)
            {

                case HLSType.Audio:     segmentInfo.path += "\\Audio";      break;
                case HLSType.Video:     segmentInfo.path += "\\Video";      break;
                case HLSType.Combined:  segmentInfo.path += "\\Combined";   break;
            }

            HLSStream.addCommand(new HLSDownloadSegmentCommand(segmentInfo));
        }

        public override string getName() { return "HLSDeconstructSegmentCommand"; }
    }
}
