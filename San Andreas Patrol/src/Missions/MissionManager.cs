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

namespace SanAndreasPatrol.Missions {
    class MissionManager {
        public static List<IMission> Missions = new List<IMission>() {
            new ActiveShooterMission()
        };

        public static IMission Mission;

        public static void Fiber() {
            KeyboardData.Register(Keys.X, OnKeyDown);
        }

        public static void OnKeyDown() {
            if (CareerManager.Career == null)
                return;

            if (Mission != null) {
                Mission.OnMissionStop();

                Mission = null;

                return;
            }

            Mission = Missions[new Random().Next(Missions.Count)];

            GameFiber.StartNew(() => Mission.OnMissionStart());
        }
    }
}
