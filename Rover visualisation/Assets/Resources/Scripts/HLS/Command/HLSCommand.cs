using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.HLS.Command
{

    public abstract class HLSCommand
    {

        // info
        protected HLSInfo info;

        // Constructors
        public HLSCommand(HLSInfo info) { this.info = info; }
        ~HLSCommand()                   { this.info = null; }

        // Abstract functions
        public abstract void execute();
        public abstract string getName();
    }
}
