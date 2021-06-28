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

[assembly: Rage.Attributes.Plugin("San Andreas Patrol", Description = "An immersive and realistic law enforcement roleplay plugin.", Author = "Chloe Ohlsson")]

namespace SanAndreasPatrol {
    public static class EntryPoint {
        public static MenuPool MenuPool = new MenuPool();

        public static List<Vehicle> Vehicles = new List<Vehicle>();

        public static bool RadarDisabled = false;

        public static Dictionary<string, int> ComponentIndex = new Dictionary<string, int>() {
            { "Hat", 0 },
            { "Masks", 1 },
            { "Hair", 2 },
            { "Torso", 3 },
            { "Legs", 4 },
            { "Parachutes", 5 },
            { "Shoes", 6 },
            { "Accessories", 7 },
            { "Undershirt", 8 },
            { "Armor", 9 },
            { "Decals", 10 },
            { "Top", 11 }
        };

        private static void Main() {
            GameFiber.StartNew(KeyboardData.Fiber);

            GameFiber.StartNew(AgencyManager.Fiber);
            GameFiber.StartNew(CareerManager.Fiber);

            while (true) {
                GameFiber.Yield();

                MenuPool.ProcessMenus();

                if(RadarDisabled)
                    NativeFunction.CallByName<int>("HIDE_HUD_AND_RADAR_THIS_FRAME");
            }

            foreach(Vehicle vehicle in Vehicles) {
                if(vehicle) {
                    vehicle.Delete();
                }
            }
        }

        public static void Print() {
            Game.Console.Print();

            File.AppendAllText("plugins/San Andreas Patrol/console.log", Environment.NewLine);
        }

        public static void Print(string text) {
            Game.Console.Print("[San Andreas Patrol] " + text);

            File.AppendAllText("plugins/San Andreas Patrol/console.log", "[San Andreas Patrol] " + text + Environment.NewLine);
        }

        public static void Print(string trace, string text) {
            Game.Console.Print("[San Andreas Patrol > " + trace + "] " + text);

            File.AppendAllText("plugins/San Andreas Patrol/console.log", "[San Andreas Patrol > " + trace + "] " + text + Environment.NewLine);
        }

        [Rage.Attributes.ConsoleCommand]
        public static void GetPlayerPosition() {
            Print();

            Print("GetPlayerPosition", "Character Position");
            Print("GetPlayerPosition", "Vector3(" + Game.LocalPlayer.Character.Position.X + ", " + Game.LocalPlayer.Character.Position.Y + ", " + Game.LocalPlayer.Character.Position.Z + ")");
            Print("GetPlayerPosition", "Rotator(" + Game.LocalPlayer.Character.Rotation.Pitch + ", " + Game.LocalPlayer.Character.Rotation.Roll + ", " + Game.LocalPlayer.Character.Rotation.Yaw + ")");


            if (Game.LocalPlayer.Character.IsInAnyVehicle(false)) {
                Vehicle vehicle = Game.LocalPlayer.Character.CurrentVehicle;

                Print();

                Print("GetPlayerPosition", "Vehicle Position");
                Print("GetPlayerPosition", "Vector3(" + vehicle.Position.X + ", " + vehicle.Position.Y + ", " + vehicle.Position.Z + ")");
                Print("GetPlayerPosition", "Rotator(" + vehicle.Rotation.Pitch + ", " + vehicle.Rotation.Roll + ", " + vehicle.Rotation.Yaw + ")");
            }
        }
    }
}
