using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SanAndreasPatrol.Agencies;

namespace SanAndreasPatrol.Radio {
    class RadioMessage {
        public string Id;

        public List<RadioMessageAgency> Agencies = new List<RadioMessageAgency>();
    }

    class RadioMessageAgency {
        public Agency Agency;
        public bool Default;

        public string Message;
    }
}
