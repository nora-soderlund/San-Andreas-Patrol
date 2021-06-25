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
    class CareerManager {
        public static readonly Keys Key = Keys.F7;

        public static List<Career> Careers = new List<Career>();

        public static Dictionary<string, List<string>> Names = new Dictionary<string, List<string>>();

        public static void Start() {
            XDocument xDocument;

            if(!File.Exists("plugins/San Andreas Patrol/careers.xml")) {
                xDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),

                    new XElement("Careers")
                );

                xDocument.Save("plugins/San Andreas Patrol/careers.xml");
            }
            else {
                Directory.CreateDirectory("plugins/San Andreas Patrol/backup");

                xDocument = XDocument.Load("plugins/San Andreas Patrol/careers.xml");

                xDocument.Save("plugins/San Andreas Patrol/backup/careers.xml");
            }

            foreach(XElement xCareer in xDocument.Elements("Career")) {
                Career career = new Career() {
                    Name = xCareer.Element("Name").Value
                };

                Careers.Add(new Career(xCareer));
            }

            xDocument = XDocument.Load("plugins/San Andreas Patrol/names.xml");

            foreach(XElement xNames in xDocument.Element("Names").Elements("Group")) {
                List<string> names = new List<string>();

                foreach(XElement xName in xNames.Elements("Name")) {
                    names.Add(xName.Value);
                }

                Names.Add(xNames.Attribute("id").Value, names);
            }

            CareerMenu.Start();
        
            Game.DisplayHelp($"Press ~{Key.GetInstructionalId()}~ to start San Andreas Patrol.");

            Keyboard.Register(Key, OnKeyDown);
            Keyboard.Register(Keys.T, OnKeyDownReset);
        }

        private static void OnKeyDown() {
            if (UIMenu.IsAnyMenuVisible || TabView.IsAnyPauseMenuVisible)
                return;

            if (Careers.Count == 0) {
                GameFiber.StartNew(CareerCreation.Start);

                return;
            }

            CareerMenu.Tab.Visible = !CareerMenu.Tab.Visible;
        }

        private static void OnKeyDownReset() {
            Game.FadeScreenIn(0);
        }
    }
}
