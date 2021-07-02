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

namespace SanAndreasPatrol.Missions {
    class ActiveShooterMission : IMission {
        public bool Active = false;

        public string Name => "Active Shooter";

        public AgencyRankType[] Ranks => new AgencyRankType[] { AgencyRankType.Officer };

        public List<uint> Notifications = new List<uint>();

        public bool Responding = false;

        public Ped Suspect;

        public Blip Blip;

        public void OnMissionStart() {
            Active = true;

            List<Ped> peds = World.GetAllPeds().Where(x => x.Exists() && !x.IsPlayer && x.IsHuman && x.RelationshipGroup != RelationshipGroup.Cop).ToList();

            Suspect = peds[new Random().Next(peds.Count)];

            Suspect.IsPersistent = true;

            Suspect.Inventory.GiveNewWeapon(WeaponHash.Pistol50, 100, true);
            
            Suspect.Tasks.Clear();

            Notifications.Add(RadioManager.Dispatch("ACTIVE_SHOOTER_START", new Dictionary<string, string>() {
                { "street", World.GetStreetName(Suspect.Position) },
                { "incident", "000000" }
            }));

            Blip = new Blip(Suspect) {
                Sprite = BlipSprite.Destination2,
                
                IsRouteEnabled = true,

                RouteColor = HudColor.Red.GetColor(),
                Color = HudColor.Red.GetColor()
            };

            Dictionary<string, string> requestReplacements = new Dictionary<string, string>() { { "unit", "6A12" }, { "street", World.GetStreetName(Game.LocalPlayer.Character.Position) } };

            RadioManager.Request("MISSION_RESPOND_CODE_3", requestReplacements, () => {
                Responding = true;

                Notifications.Add(RadioManager.Dispatch("MISSION_RESPOND_CODE_3", requestReplacements));

                Game.DisplaySubtitle("Respond to the ~r~area~w~ with lights and sirens.");
            });

            RadioManager.Request("MISSION_RESPOND_CODE_2", requestReplacements, () => {
                Responding = true;

                Notifications.Add(RadioManager.Dispatch("MISSION_RESPOND_CODE_2", requestReplacements));

                Game.DisplaySubtitle("Respond to the ~r~area~w~ without lights and sirens.~n~Upgrade to lights and sirens in the radio menu.");

                RadioManager.Request("MISSION_RESPOND_CODE_3_UPGRADE", requestReplacements, () => {
                    Notifications.Add(RadioManager.Dispatch("MISSION_RESPOND_CODE_3_UPGRADE", requestReplacements));

                    Game.DisplaySubtitle("Respond to the ~r~area~w~ with lights and sirens.");
                });
            });

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

            if (Blip != null && Blip.Exists())
                Blip.Delete();

            Blip = null;

            Responding = false;
            Suspect = null;

            RadioManager.ClearRequests();

            foreach (uint notification in Notifications)
                Game.RemoveNotification(notification);

            Notifications.Clear();
        }
    }
}
