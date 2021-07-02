using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

using Rage;
using Rage.Native;
using Rage.Attributes;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Career;
using SanAndreasPatrol.Extras;

namespace SanAndreasPatrol.Missions {
    interface IMission {
        string Name { get; }
        AgencyRankType[] Ranks { get; }

        void OnMissionStart();

        void OnMissionStop();
    }
}
