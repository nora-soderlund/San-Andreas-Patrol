using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

using SanAndreasPatrol.Career.Creation;
using SanAndreasPatrol.Career.Menu;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Agencies.Stations;

namespace SanAndreasPatrol.Career {
    class CareerManager {
        public static readonly Keys Key = Keys.F7;

        public static Career Career = null;

        public static List<Career> Careers = new List<Career>();

        public static Dictionary<string, List<string>> Names = new Dictionary<string, List<string>>();

        public static Vehicle Vehicle;
        public static List<Blip> Blips = new List<Blip>();

        public static void Fiber() {
            Directory.CreateDirectory("plugins/San Andreas Patrol/careers/backup/");

            foreach (string file in Directory.GetFiles("plugins/San Andreas Patrol/careers/")) {
                string id = Path.GetFileNameWithoutExtension(file);

                if (File.Exists("plugins/San Andreas Patrol/careers/backup/" + id + ".xml"))
                    File.Delete("plugins/San Andreas Patrol/careers/backup/" + id + ".xml");

                File.Copy(file, "plugins/San Andreas Patrol/careers/backup/" + id + ".xml");

                Careers.Add(new Career(id));
            }

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/data/names.xml");

            foreach(XElement xNames in xDocument.Element("Names").Elements("Group")) {
                List<string> names = new List<string>();

                foreach(XElement xName in xNames.Elements("Name")) {
                    names.Add(xName.Value);
                }

                Names.Add(xNames.Attribute("id").Value, names);
            }

            GameFiber.StartNew(CareerCreation.Fiber);
            GameFiber.StartNew(CareerMenu.Fiber);

            KeyboardData.Register(Key, OnKeyDown);
            KeyboardData.Register(Keys.T, OnKeyDownReset);

            if(PluginSettings.StartOnLoad) {
                Career career = Careers.Find(x => x.Id == PluginSettings.PreviousCareer);

                if (career != null)
                    Start(career);
            }
            else
                Game.DisplayHelp($"Press ~{Key.GetInstructionalId()}~ to start San Andreas Patrol.");
        }

        public static void Start(Career career) {
            if(Career != null)
                Stop();

            Career = career;

            Game.FadeScreenOut(1000, true);

            Career.Character.Apply();

            AgencyOutfit agencyOutfit = Career.Agency.Outfits.Find(x => x.Type == AgencyOutfitType.Patrol && x.Gender == Career.Character.Gender);
            agencyOutfit.Apply(Game.LocalPlayer.Character);


            List<AgencyStationSpawn> agencyStationSpawns = Career.Station.Spawns.Where(x => x.Type == "parking").ToList();
            AgencyStationSpawn agencyStationParkingSpawn = agencyStationSpawns[new Random().Next(agencyStationSpawns.Count)];

            foreach (Vehicle blockingVehicle in World.GetAllVehicles().Where(x => x.Exists() && x.DistanceTo(agencyStationParkingSpawn.Position) < 4.0f))
                blockingVehicle.Delete();

            Vehicle vehicle = new Vehicle(new Model("SHERIFF"), agencyStationParkingSpawn.Position);
            vehicle.Rotation = agencyStationParkingSpawn.Rotation;
            EntryPoint.Vehicles.Add(vehicle);

            new Blip(vehicle) {
                Sprite = BlipSprite.GangVehiclePolice
            };

            Game.LocalPlayer.Character.Position = agencyStationParkingSpawn.Position;

            Game.LocalPlayer.Character.WarpIntoVehicle(vehicle, -1);

            foreach (Agency agency in AgencyManager.Agencies) {
                int number = 0;

                foreach (AgencyStation agencyStation in agency.Stations) {
                    AgencyStationSpawn agencyStationSpawn = agencyStation.Spawns.FirstOrDefault(x => x.Type == "entrance");

                    if (agencyStationSpawn == null)
                        continue;

                    number++;

                    Blip blip = new Blip(agencyStationSpawn.Position, 100.0f) {
                        NumberLabel = number,
                        Sprite = BlipSprite.PoliceStation2,
                        Color = (agency.Id == Career.Agency.Id)?(HudColor.White.GetColor()):(HudColor.Grey.GetColor())
                    };

                    blip.Name = agency.Name;

                    Blips.Add(blip);
                }
            }

            Game.FadeScreenIn(1000);
        }

        public static void Stop() {
            if (Career == null)
                return;

            Career.Save();

            Career = null;

            Game.LocalPlayer.IsIgnoredByPolice = false;
            Game.LocalPlayer.Character.RelationshipGroup = RelationshipGroup.Player;

            foreach (Blip blip in Blips.Where(x => x.Exists()))
                blip.Delete();

            Blips.Clear();

            if (Vehicle != null && Vehicle.Exists())
                Vehicle.Delete();

            Vehicle = null;
        }

        public static Vehicle SpawnVehicle(CareerVehicle vehicle) {
            if (Vehicle != null)
                DestroyVehicle();

            foreach (Vehicle blockingVehicle in World.GetAllVehicles().Where(x => x.Exists() && x.DistanceTo(vehicle.Position) < 4.0f))
                blockingVehicle.Delete();

            Vehicle = new Vehicle(new Model(vehicle.Model), vehicle.Position);
            Vehicle.Rotation = vehicle.Rotation;

            return Vehicle;
        }

        public static void DestroyVehicle() {
            if (Vehicle == null)
                return;

            if (Vehicle.Exists())
                Vehicle.Delete();

            Vehicle = null;
        }

        private static void OnKeyDown() {
            if (UIMenu.IsAnyMenuVisible || TabView.IsAnyPauseMenuVisible)
                return;

            if (CareerCreation.Active)
                return;

            if (Careers.Count == 0) {
                GameFiber.StartNew(CareerCreation.Start);

                return;
            }

            CareerMenu.Toggle();
        }

        private static void OnKeyDownReset() {
            Game.FadeScreenIn(0);
        }
    }
}
