using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Native;
using Rage.Attributes;

using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Stations;

using SanAndreasPatrol.Career.Creation;

namespace SanAndreasPatrol.Career {
    class CareerCreation {
        public static Vector3 OriginalPosition;
        public static bool CareerMenuEnabled;

        public static Agency Agency;
        public static Station Station;
        public static string Difficulty;

        public static Camera Camera;

        public static void Start() {
            CareerMenuEnabled = CareerMenu.Tab.Visible;
            OriginalPosition = Game.LocalPlayer.Character.Position;

            Game.FadeScreenOut(1000, true);

            Camera = new Camera(true);

            CareerMenu.Tab.Visible = false;

            Game.LocalPlayer.HasControl = false;
            Game.LocalPlayer.Character.IsVisible = false;

            EntryPoint.RadarDisabled = true;

            CareerAgencyCreation.Start();

            Game.FadeScreenIn(1000);
        }

        public static void Dispose() {
            Game.Console.Print("CareerCreation.Dispose");

            Game.FadeScreenOut(1000, true);

            if(Camera.IsValid())
                Camera.Delete();

            CareerMenu.Tab.Visible = CareerMenuEnabled;
            Game.LocalPlayer.Character.Position = OriginalPosition;

            Game.LocalPlayer.HasControl = true;
            Game.LocalPlayer.Character.IsVisible = true;

            EntryPoint.RadarDisabled = false;

            Game.FadeScreenIn(1000);
        }
    }
}
