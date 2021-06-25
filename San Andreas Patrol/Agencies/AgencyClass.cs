using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol.Agencies {
    class Agency {
        public string Id;

        public bool Default;
        public bool Disabled;

        public string Name;
        public string Description;
        public string Motto;
        public string Abbreviation;
        public string Formed;

        public int SwornEmployees;
        public int UnswornEmployees;

        public List<string> Ranks = new List<string>();

        public List<AgencyOutfit> Outfits = new List<AgencyOutfit>();

        public Dictionary<string, string> Images = new Dictionary<string, string>();
    }
}
