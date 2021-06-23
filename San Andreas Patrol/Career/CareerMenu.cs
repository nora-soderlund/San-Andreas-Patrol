using System;
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
    class CareerMenu {
        public static TabView Tab;

        private static TabMissionSelectItem AgenciesTab;

        public static void Initialize() {
            Tab = new TabView("San Andreas Patrol");

            Tab.Name = " "; // name
            Tab.Money = " ";
            Tab.MoneySubtitle = " "; // agency


            List<TabItem> items = new List<TabItem>();

            TabTextItem newCareer = new TabTextItem("Start a New Career", "New Career", $"Press enter to start a new San Andreas Patrol career.");

            newCareer.Activated += (s, e) => new CareerCreation();
            
            items.Add(newCareer);

            Tab.AddTab(new TabSubmenuItem("Career", items));



            List<MissionInformation> agencies = new List<MissionInformation>();
            
            foreach(XElement element in CareerEntryPoint.Agencies.Element("Agencies").Elements("Agency")) {
                Game.Console.Print(element.Attribute("id").Value);

                if (element.Attribute("disabled") != null)
                    continue;

                string name = element.Element("Name").Value;
                string description = element.Element("Description").Value ?? "";

                List<Tuple<string, string>> lines = new List<Tuple<string, string>>();

                if(element.Element("Motto").Value != null && element.Element("Formed").Value != null) {
                    lines.Add(new Tuple<string, string>("Motto", element.Element("Motto").Value));
                    lines.Add(new Tuple<string, string>("Formed", element.Element("Formed").Value));

                    lines.Add(new Tuple<string, string>("", ""));
                }

                if(!element.Element("Employees").IsEmpty) {
                    lines.Add(new Tuple<string, string>("Sworn Employees", int.Parse(element.Element("Employees").Element("Sworn").Value).ToString("N0")));
                    lines.Add(new Tuple<string, string>("Unsworn Employees", int.Parse(element.Element("Employees").Element("Unsworn").Value).ToString("N0")));

                    lines.Add(new Tuple<string, string>("", ""));
                }

                if(element.Attribute("disabled") == null)
                    lines.Add(new Tuple<string, string>("Stations", CareerEntryPoint.Stations.Element("Stations").Elements("Station").Count(x => x.Attribute("agency").Value == element.Attribute("id").Value).ToString("N0")));

                agencies.Add(new MissionInformation(name, description, lines.ToArray()) {
                    Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + element.Element("Images").Elements("Image").Where(x => x.Attribute("id").Value == "card").FirstOrDefault().Value))
                });

                foreach(XElement station in CareerEntryPoint.Stations.Element("Stations").Elements("Station").Where(x => x.Attribute("agency").Value == element.Attribute("id").Value)) {
                    Game.Console.Print(station.Attribute("id").Value);
                    
                    agencies.Add(new MissionInformation(
                        "\t" + station.Element("Name").Value,
                        station.Element("Description").Value ?? "",

                        new Tuple<string, string>[] {
                            new Tuple<string, string>("Type", station.Element("Type").Value)
                        }
                    ) {
                        Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + station.Element("Images").Elements("Image").Where(x => x.Attribute("id").Value == "card").FirstOrDefault().Value))
                    });
                }
            }

            Tab.AddTab(AgenciesTab = new TabMissionSelectItem("Agencies", agencies));

            GameFiber.StartNew(OnFiberStart);
        }

        private static void OnFiberStart() {
            Game.RawFrameRender += (s, e) => Tab.DrawTextures(e.Graphics);

            while(true) {
                GameFiber.Yield();

                Tab.Update();
            }
        }
    }
}
