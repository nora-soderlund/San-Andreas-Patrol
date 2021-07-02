using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol.Agencies {
    enum AgencyOutfitType {
        Formal = 0,
        Patrol
    }

    class AgencyOutfit {
        public string Id;
        public AgencyOutfitType Type;
        public Gender Gender;

        public List<AgencyOutfitPart> Parts = new List<AgencyOutfitPart>();

        public static Dictionary<string, AgencyOutfitType> Types = new Dictionary<string, AgencyOutfitType>() {
            { "formal", AgencyOutfitType.Formal },
            { "patrol", AgencyOutfitType.Patrol }
        };

        public void Apply(Ped ped) {
            foreach (AgencyOutfitPart agencyOutfitPart in Parts)
                ped.SetVariation(EntryPoint.ComponentIndex[agencyOutfitPart.Id], agencyOutfitPart.Drawable, agencyOutfitPart.Texture);
        }
    }

    class AgencyOutfitPart {
        public string Id;
        public int Drawable;
        public int Texture;
    }
}
