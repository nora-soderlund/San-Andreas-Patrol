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

        public static void Fiber() {
            Directory.CreateDirectory("plugins/San Andreas Patrol/careers/backup/");

            foreach (string file in Directory.GetFiles("plugins/San Andreas Patrol/careers/")) {
                string id = Path.GetFileNameWithoutExtension(file);

                if (File.Exists("plugins/San Andreas Patrol/careers/backup/" + id + ".xml"))
                    File.Delete("plugins/San Andreas Patrol/careers/backup/" + id + ".xml");

                File.Copy(file, "plugins/San Andreas Patrol/careers/backup/" + id + ".xml");

                Careers.Add(new Career(id));
            }

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/names.xml");

            foreach(XElement xNames in xDocument.Element("Names").Elements("Group")) {
                List<string> names = new List<string>();

                foreach(XElement xName in xNames.Elements("Name")) {
                    names.Add(xName.Value);
                }

                Names.Add(xNames.Attribute("id").Value, names);
            }

            GameFiber.StartNew(CareerCreation.Fiber);
            GameFiber.StartNew(CareerMenu.Fiber);
        
            Game.DisplayHelp($"Press ~{Key.GetInstructionalId()}~ to start San Andreas Patrol.");

            KeyboardData.Register(Key, OnKeyDown);
            KeyboardData.Register(Keys.T, OnKeyDownReset);
        }

        public static void Start(Career career) {
            if(Career != null) {
                Career.Save();

                Career = null;

                Game.LocalPlayer.IsIgnoredByPolice = false;
            }

            Career = career;

            Game.FadeScreenOut(1000, true);

            Game.LocalPlayer.IsIgnoredByPolice = true;

            AgencyStationSpawn agencyStationSpawn = Career.Station.Spawns.Where(x => x.Type == "parking").FirstOrDefault(x => x.Step == "start");
            AgencyStationSpawn agencyStationSpawnFinish = Career.Station.Spawns.Where(x => x.Type == "parking").FirstOrDefault(x => x.Step == "finish");

            foreach (Vehicle tempVehicle in World.GetAllVehicles().Where(x => x.Exists() && x.DistanceTo(agencyStationSpawn.Position) < 5.0f)) {
                tempVehicle.Delete();
            }

            Vehicle vehicle = new Vehicle(new Model("POLICE"), agencyStationSpawn.Position);
            vehicle.Rotation = agencyStationSpawn.Rotation;
            EntryPoint.Vehicles.Add(vehicle);

            Game.LocalPlayer.Character.Position = agencyStationSpawn.Position;

            Game.LocalPlayer.Character.WarpIntoVehicle(vehicle, -1);

            AgencyStationCamera agencyStationCamera = Career.Station.Cameras.FirstOrDefault(x => x.Type == "parking");

            Task driveToPosition = Game.LocalPlayer.Character.Tasks.DriveToPosition(agencyStationSpawnFinish.Position, 6.0f, VehicleDrivingFlags.StopAtDestination);

            Game.FadeScreenIn(1000);

            driveToPosition.WaitForCompletion(8000);

            Game.LocalPlayer.Character.Tasks.Clear();
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
