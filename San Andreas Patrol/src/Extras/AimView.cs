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

namespace SanAndreasPatrol.Extras {
    class AimView {
        public static void Fiber() {
            while(true) {
                GameFiber.Yield();

                Game.LocalPlayer.WantedLevel = 0;
                Game.LocalPlayer.IsIgnoredByPolice = true;
                Game.LocalPlayer.Character.RelationshipGroup = RelationshipGroup.Cop;

                foreach (Ped ped in World.GetAllPeds().Where(x => x.Exists() && !x.IsPlayer && x.RelationshipGroup == RelationshipGroup.Cop))
                    ped.CanAttackFriendlies = false;
            }
        }
    }
}
