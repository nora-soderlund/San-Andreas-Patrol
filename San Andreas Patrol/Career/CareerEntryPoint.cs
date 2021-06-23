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

namespace SanAndreasPatrol.Career {
    class CareerEntryPoint : IEntryPoint {
        public static readonly Keys Key = Keys.F7;

        public static XDocument Agencies;
        public static XDocument Stations;
        public static XDocument Careers;

        public void OnFiberStart() {
            Agencies = XDocument.Load("plugins/San Andreas Patrol/agencies.xml");
            Stations = XDocument.Load("plugins/San Andreas Patrol/stations.xml");
            
            if(!File.Exists("plugins/San Andreas Patrol/careers.xml")) {
                Careers = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),

                    new XElement("Careers")
                );

                Careers.Save("plugins/San Andreas Patrol/careers.xml");
            }
            else {
                Directory.CreateDirectory("plugins/San Andreas Patrol/backup");

                Careers = XDocument.Load("plugins/San Andreas Patrol/careers.xml");

                Careers.Save("plugins/San Andreas Patrol/backup/careers.xml");
            }

            CareerMenu.Initialize();
        
            Game.DisplayHelp($"Press ~{Key.GetInstructionalId()}~ to start San Andreas Patrol.");

            Keyboard.Register(Key, OnKeyDown);

            while(true) {
                GameFiber.Yield();
            }

            Careers.Save("plugins/San Andreas Patrol/careers.xml");
        }

        private static void OnKeyDown() {
            if (UIMenu.IsAnyMenuVisible || TabView.IsAnyPauseMenuVisible)
                return;

            CareerMenu.Tab.Visible = !CareerMenu.Tab.Visible;
        }
    }
}
