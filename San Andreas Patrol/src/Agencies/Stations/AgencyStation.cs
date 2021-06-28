using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol.Agencies.Stations {
    class AgencyStation {
        public string Id;

        public bool Default;
        public bool Disabled;

        public string Name;
        public string Description;
        public string Type;

        public List<AgencyStationCamera> Cameras = new List<AgencyStationCamera>();

        public List<AgencyStationSpawn> Spawns = new List<AgencyStationSpawn>();

        public Dictionary<string, string> Images = new Dictionary<string, string>();
    }

    class AgencyStationCamera {
        public string Type;

        public Vector3 Position;
        public Rotator Rotation;
    }

    class AgencyStationSpawn {
        public string Type;
        public string Step;

        public Vector3 Position;
        public Rotator Rotation;
    }
}
