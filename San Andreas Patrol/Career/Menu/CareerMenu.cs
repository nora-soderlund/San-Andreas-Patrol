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

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Agencies.Stations;
using SanAndreasPatrol.Career.Creation;

namespace SanAndreasPatrol.Career.Menu {
    class CareerMenu {
        public static TabView Tab;

        private static TabMissionSelectItem AgenciesTab;

        public static void Start() {
            Tab = new TabView("San Andreas Patrol");

            Tab.Name = " "; // name
            Tab.Money = " ";
            Tab.MoneySubtitle = " "; // agency


            List<TabItem> items = new List<TabItem>();

            TabTextItem newCareer = new TabTextItem("Start a New Career", "New Career", $"Press enter to start a new San Andreas Patrol career.");

            newCareer.Activated += (s, e) => GameFiber.StartNew(CareerCreation.Start);
            
            items.Add(newCareer);

            Tab.AddTab(new TabSubmenuItem("Career", items));



            List<MissionInformation> agencies = new List<MissionInformation>();

            foreach(Agency agency in AgencyManager.Agencies.Where(x => x.Disabled != true)) {
                List<Tuple<string, string>> information = new List<Tuple<string, string>>();

                if (agency.Motto.Length != 0 && agency.Formed.Length != 0) {
                    information.Add(new Tuple<string, string>("Motto", agency.Motto));
                    information.Add(new Tuple<string, string>("Formed", agency.Formed));

                    information.Add(new Tuple<string, string>("", ""));
                }

                if (agency.SwornEmployees != 0)
                    information.Add(new Tuple<string, string>("Sworn Employees", agency.SwornEmployees.ToString("N0")));

                if (agency.UnswornEmployees != 0)
                    information.Add(new Tuple<string, string>("Unsworn Employees", agency.UnswornEmployees.ToString("N0")));

                if (agency.SwornEmployees != 0 || agency.UnswornEmployees != 0)
                    information.Add(new Tuple<string, string>("", ""));

                information.Add(new Tuple<string, string>("Stations", agency.Stations.Count.ToString("N0")));

                agencies.Add(new MissionInformation(agency.Name, agency.Description, information) {
                    Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + agency.Images["card"]))
                });

                foreach (AgencyStation station in agency.Stations) {
                    agencies.Add(new MissionInformation(
                        "\t" + station.Name,
                        station.Description,

                        new Tuple<string, string>[] {
                            new Tuple<string, string>("Type", station.Type)
                        }
                    ) {
                        Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + station.Images["card"]))
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
