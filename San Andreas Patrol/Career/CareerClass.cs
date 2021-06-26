using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Agencies.Stations;

namespace SanAndreasPatrol.Career {
    class Career {
        public Agency Agency;
        public AgencyStation Station;

        public CareerDifficulty Difficulty;

        public CareerCharacter Character = new CareerCharacter();

        public Career() {

        }

        public Career(XElement xElement) {

        }
    }

    enum CareerDifficulty {
        Normal = 0, Realistic
    }

    class CareerCharacter {
        public Gender Gender = Gender.Unisex;

        public int Hair;

        public int Glasses;
        public int GlassesTexture;
    }
}
