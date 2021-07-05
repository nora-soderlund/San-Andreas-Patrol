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

using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Career;
using SanAndreasPatrol.Extras;
using SanAndreasPatrol.Radio;
using SanAndreasPatrol.Radio.Tasks.Missions;

namespace SanAndreasPatrol.Missions {
    class ActiveShooterMission : IMission {
        public bool Active = false;

        public string Name => "Active Shooter";

        public AgencyRankType[] Ranks => new AgencyRankType[] { AgencyRankType.Officer };

        public List<uint> Notifications = new List<uint>();

        public Ped Suspect;

        public void OnMissionStart() {
            Active = true;

            Suspect = MissionManager.GetRandomSuspect();

            Suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol50, 100, true);

            MissionManager.BlipPool.Add(new Blip(World.GetNextPositionOnStreet(Suspect.Position)) {
                Sprite = BlipSprite.Unknown9,

                Color = System.Drawing.Color.FromArgb(128, 255, 0, 0),

                IsRouteEnabled = true
            });

            RadioManager.StartTask(new RadioDispatchTask("ACTIVE_SHOOTER", true, true, new Dictionary<string, string>() {
                { "unit", "6A12" },
                { "street", World.GetStreetName(Game.LocalPlayer.Character.Position) },
                { "incident", "000000" },

                { "suspect.street", World.GetStreetName(Suspect.Position) }
            }));

            Ped targetPed = null;

            while (Active && Suspect.Exists() && Suspect.IsAlive) {
                List<Ped> targetPeds = World.GetAllPeds().Where(x => x.Exists() && x.IsHuman && !x.IsPlayer && x != Suspect).OrderBy(x => x.DistanceTo(Suspect.Position)).ToList();

                if (targetPeds.Count != 0 && targetPed != targetPeds[0]) {
                    targetPed = targetPeds[0];

                    Suspect.Tasks.FightAgainst(targetPed);
                }

                GameFiber.Yield();
            }

            OnMissionStop();
        }

        public void OnMissionStop() {
            if (!Active)
                return;

            Active = false;

            Suspect = null;

            RadioManager.ClearRequests();

            foreach (uint notification in Notifications)
                Game.RemoveNotification(notification);

            Notifications.Clear();
        }
    }
}
