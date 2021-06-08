using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS
{
    public enum HLSType
    {

        None =          0x00,

        Video =         0x01,
        Audio =         0x02,
        Combined =      0x03,

        PreVideo =      0x04,
        PreAudio =      0x05,
        PreCombined =   0x06,
        PreSeperate =   0x07
    }
}
