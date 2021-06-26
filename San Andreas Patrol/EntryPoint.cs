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
            GameFiber.StartNew(KeyboardData.Main);

            GameFiber.StartNew(AgencyManager.Main);
            GameFiber.StartNew(CareerManager.Main);

            while (true) {
                GameFiber.Yield();

                MenuPool.ProcessMenus();

                if(RadarDisabled)
                    NativeFunction.CallByName<int>("HIDE_HUD_AND_RADAR_THIS_FRAME");
            }
        }
    }
}
