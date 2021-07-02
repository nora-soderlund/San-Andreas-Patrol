using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using SanAndreasPatrol.Agencies.Stations;

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

        public List<AgencyRank> Ranks = new List<AgencyRank>();

        public List<AgencyOutfit> Outfits = new List<AgencyOutfit>();

        public Dictionary<string, string> Images = new Dictionary<string, string>();

        public List<AgencyStation> Stations = new List<AgencyStation>();

        public AgencyStation GetDefaultStation() {
            return Stations.FirstOrDefault(x => x.Default) ?? Stations.FirstOrDefault();
        }

        public AgencyStation GetStationById(string id) {
            return Stations.Find(x => x.Id == id);
        }

        public AgencyStation GetStationByName(string name) {
            return Stations.Find(x => x.Name == name);
        }

        public List<string> GetStationNames() {
            List<string> names = new List<string>();

            foreach (AgencyStation station in Stations.Where(x => x.Disabled != true))
                names.Add(station.Name);

            return names;
        }
    }

    class AgencyRank {
        public int Id;
        public string Name;
        public AgencyRankType Type;
        public int Step;
        public bool Default;
    }

    enum AgencyRankType {
        Officer = 0,
        Detective,
        Sergeant,
        Lieutenant,
        Captain,
        Commander,
        Chief
    }
}
