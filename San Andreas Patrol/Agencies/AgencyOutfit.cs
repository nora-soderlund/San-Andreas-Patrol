using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanAndreasPatrol.Agencies {
    enum AgencyOutfitType {
        Formal = 0
    }

    class AgencyOutfit {
        public string Id;
        public AgencyOutfitType Type;
        public Gender Gender;

        public List<AgencyOutfitPart> Parts = new List<AgencyOutfitPart>();

        public static Dictionary<string, AgencyOutfitType> Types = new Dictionary<string, AgencyOutfitType>() {
            { "formal", AgencyOutfitType.Formal }
        };
    }

    class AgencyOutfitPart {
        public string Id;
        public int Drawable;
        public int Texture;
    }
}
