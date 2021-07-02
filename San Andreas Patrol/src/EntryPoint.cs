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
using SanAndreasPatrol.Missions;
using SanAndreasPatrol.Radio;

[assembly: Rage.Attributes.Plugin("San Andreas Patrol", Description = "An immersive and realistic law enforcement roleplay plugin.", Author = "Chloe Ohlsson", ExitPoint = "SanAndreasPatrol.ExitPoint.Exit")]

namespace SanAndreasPatrol {
    public static class EntryPoint {
        public static MenuPool MenuPool = new MenuPool();

        public static List<Vehicle> Vehicles = new List<Vehicle>();

        public static bool RadarDisabled = false;

        public static Dictionary<string, int> ComponentIndex = new Dictionary<string, int>() {
            { "hat", 0 },
            { "mask", 1 },
            { "hair", 2 },
            { "upperskin", 3 },
            { "pants", 4 },
            { "parachute", 5 },
            { "shoes", 6 },
            { "accessories", 7 },
            { "undercoat", 8 },
            { "armor", 9 },
            { "decals", 10 },
            { "top", 11 }
        };

        private static void Main() {
            PluginSettings.Fiber();

            GameFiber.StartNew(KeyboardData.Fiber);

            GameFiber.StartNew(AgencyManager.Fiber);
            GameFiber.StartNew(CareerManager.Fiber);
            GameFiber.StartNew(AimView.Fiber);
            GameFiber.StartNew(RadioManager.Fiber);
            GameFiber.StartNew(MissionManager.Fiber);

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

        [Rage.Attributes.ConsoleCommand]
        public static void SaveVehiclePosition() {
            if (!Game.LocalPlayer.Character.IsInAnyVehicle(false))
                return;

            Vehicle vehicle = Game.LocalPlayer.Character.CurrentVehicle;

            Print();

            Print("SaveVehiclePosition", "<Spawn type=\"spawn_type\">");
            Print("SaveVehiclePosition", "    <Position breadth=\"" + vehicle.Position.X + "\" height=\"" + vehicle.Position.Y + "\" depth=\"" + vehicle.Position.Z + "\"/>");
            Print("SaveVehiclePosition", "    <Rotation breadth=\"" + vehicle.Rotation.Pitch + "\" height=\"" + vehicle.Rotation.Roll + "\" depth=\"" + vehicle.Rotation.Yaw + "\"/>");
            Print("SaveVehiclePosition", "</Spawn>");
        }

        [Rage.Attributes.ConsoleCommand]
        public static void SavePlayerPosition() {
            Print();

            Print("SaveVehiclePosition", "<Spawn type=\"spawn_type\">");
            Print("SaveVehiclePosition", "    <Position breadth=\"" + Game.LocalPlayer.Character.Position.X + "\" height=\"" + Game.LocalPlayer.Character.Position.Y + "\" depth=\"" + Game.LocalPlayer.Character.Position.Z + "\"/>");
            Print("SaveVehiclePosition", "    <Rotation breadth=\"" + Game.LocalPlayer.Character.Rotation.Pitch + "\" height=\"" + Game.LocalPlayer.Character.Rotation.Roll + "\" depth=\"" + Game.LocalPlayer.Character.Rotation.Yaw + "\"/>");
            Print("SaveVehiclePosition", "</Spawn>");
        }
    }
}
