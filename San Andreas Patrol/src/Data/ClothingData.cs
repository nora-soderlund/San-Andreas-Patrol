using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Native;
using Rage.Attributes;

using SanAndreasPatrol.Career;
using SanAndreasPatrol.Agencies;

namespace SanAndreasPatrol {
    enum Gender {
        Unisex = 0, Male, Female
    }

    class Data {
        public static Dictionary<string, AgencyRankType> Ranks = new Dictionary<string, AgencyRankType>() {
            { "officer", AgencyRankType.Officer },
            { "sergeant", AgencyRankType.Sergeant },
            { "lieutenant", AgencyRankType.Lieutenant },
            { "detective", AgencyRankType.Detective },
            { "captain", AgencyRankType.Captain },
            { "commander", AgencyRankType.Commander },
            { "chief", AgencyRankType.Chief }
        };

        public static Dictionary<string, Gender> Genders = new Dictionary<string, Gender>() {
            { "unisex", Gender.Unisex },
            { "male", Gender.Male },
            { "female", Gender.Female }
        };

        public static Gender GetGenderByName(string name) {
            return Genders[name.ToLower()];
        }
    }
}
