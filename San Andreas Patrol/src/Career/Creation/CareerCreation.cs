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

using SanAndreasPatrol.Career.Menu;
using SanAndreasPatrol.Career.Creation.Steps;

namespace SanAndreasPatrol.Career.Creation {
    class CareerCreation {
        public static bool Active;

        public static Vector3 OriginalPosition;

        public static Career Career;

        public static UIMenu Menu;
        public static Camera Camera;

        public static void Fiber() {
            Menu = new UIMenu("Career", "");

            EntryPoint.MenuPool.Add(Menu);
        }

        public static void Start() {
            Active = true;

            Career = new Career();

            OriginalPosition = Game.LocalPlayer.Character.Position;

            Game.FadeScreenOut(1000, true);

            Camera = new Camera(true);

            if(CareerMenu.TabView.Visible)
                CareerMenu.TabView.Visible = false;

            Game.LocalPlayer.HasControl = false;
            Game.LocalPlayer.Character.IsVisible = false;

            EntryPoint.RadarDisabled = true;

            CareerAgencyCreation.Start();

            Game.FadeScreenIn(1000);
        }

        public static void Finish() {
            Dispose();

            Career.Save();

            CareerManager.Careers.Add(Career);

            if (CareerMenu.TabView.Visible)
                CareerMenu.TabView.Visible = false;

            CareerManager.Start(Career);
        }

        public static void Dispose() {
            if (Camera.IsValid())
                Camera.Delete();

            Game.LocalPlayer.Character.Position = OriginalPosition;

            Game.LocalPlayer.HasControl = true;
            Game.LocalPlayer.Character.IsVisible = true;

            EntryPoint.RadarDisabled = false;

            Active = false;
        }
    }
}
