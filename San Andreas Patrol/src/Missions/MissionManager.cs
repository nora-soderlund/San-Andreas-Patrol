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

        public static List<Ped> PedPool = new List<Ped>();
        public static List<Blip> BlipPool = new List<Blip>();

        public static void Fiber() {
            KeyboardData.Register(Keys.X, OnKeyDown);
        }

        public static Ped GetRandomSuspect() {
            List<Ped> peds = World.GetAllPeds().Where(x => x.Exists() && !x.IsPlayer && x.IsHuman && x.RelationshipGroup != RelationshipGroup.Cop).ToList();

            Ped ped = peds[new Random().Next(peds.Count)];

            ped.Tasks.Clear();
            ped.IsPersistent = true;

            return ped;
        }

        public static void Start(IMission mission) {
            Stop();

            Mission = mission;

            GameFiber.StartNew(() => Mission.OnMissionStart());
        }

        public static void Stop() {
            foreach (Blip blip in BlipPool.Where(x => x.Exists()))
                blip.Delete();

            BlipPool.Clear();

            foreach(Ped ped in PedPool.Where(x => x.Exists()))
                ped.Tasks.Clear();

            PedPool.Clear();

            if (Mission != null) {
                Mission.OnMissionStop();

                Mission = null;

                return;
            }
        }

        public static void OnKeyDown() {
            if (CareerManager.Career == null)
                return;

            Start(Missions[new Random().Next(Missions.Count)]);
        }
    }
}
