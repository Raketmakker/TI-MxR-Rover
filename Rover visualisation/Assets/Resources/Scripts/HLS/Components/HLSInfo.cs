using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS
{
    public class HLSInfo
    {

        public HLSType type;

        public string url;
        public string filename;
        public string path;

        public string extension;
        public string extensionForced;

        public int index;

        public HLSInfo(HLSInfo info = null)
        {

            if (info != null)
            {

                this.type               = info.type;

                this.url                = info.url;
                this.filename           = info.filename;
                this.path               = info.path;

                this.extension          = info.extension;
                this.extensionForced    = info.extensionForced;

                this.index              = info.index;
            }
        }

        public string getCombinedFilename() { return (this.filename + this.index + this.extension); }
    }
}
