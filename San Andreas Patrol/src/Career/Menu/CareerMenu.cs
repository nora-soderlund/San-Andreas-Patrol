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
        public static TabView TabView;

        private static TabSubmenuItem tabCareers;
        private static TabMissionSelectItem tabAgencies;
        private static TabSubmenuItem tabSettings;

        public static void Fiber() {
            TabView = new TabView("San Andreas Patrol") {
                Name = " ",

                Money = " ",
                MoneySubtitle = " "
            };

            List<UIMenuItem> pluginSettings = new List<UIMenuItem>();
            UIMenuCheckboxItem startOnLoad = new UIMenuCheckboxItem("Start career on plugin load", PluginSettings.StartOnLoad, "Whether to start your previous career when the plugin is loaded or not.");
            startOnLoad.CheckboxEvent += (UIMenuCheckboxItem sender, bool Checked) => {
                PluginSettings.StartOnLoad = Checked;
                
                PluginSettings.Save();
            };
            pluginSettings.Add(startOnLoad);

            List<UIMenuItem> careerSettings = new List<UIMenuItem>();
            UIMenuCheckboxItem progressiveWorldTime = new UIMenuCheckboxItem("Enabled progressive world time", PluginSettings.ProgressiveWorldTime, "Whether to progress the world time alongside with your career.");
            startOnLoad.CheckboxEvent += (UIMenuCheckboxItem sender, bool Checked) => {
                PluginSettings.ProgressiveWorldTime = Checked;

                PluginSettings.Save();
            };
            careerSettings.Add(progressiveWorldTime);

            tabCareers = new TabSubmenuItem("Career", new List<TabItem>());
            tabAgencies = new TabMissionSelectItem("Agencies", new List<MissionInformation>());
            tabSettings = new TabSubmenuItem("Settings", new List<TabItem>() {
                (TabItem)new TabInteractiveListItem("Plugin Settings", pluginSettings),
                (TabItem)new TabInteractiveListItem("Career Settings", careerSettings)
            });

            TabView.AddTab(tabCareers);
            TabView.AddTab(tabAgencies);
            TabView.AddTab(tabSettings);

            Game.RawFrameRender += (s, e) => TabView.DrawTextures(e.Graphics);

            while (true) {
                GameFiber.Yield();

                TabView.Update();
            }
        }

        public static void Start() {
            if (TabView.Visible == true)
                return;

            tabCareers.Items.Clear();

            foreach(Career career in CareerManager.Careers) {
                TabTextItem tabCareerItem = new TabTextItem(career.Firstname + " " + career.Lastname, "Unnamed Career", "Press enter to load this career.");

                tabCareerItem.Activated += (s, e) => {
                    TabView.Visible = false;

                    CareerManager.Start(career);
                };

                tabCareers.Items.Add(tabCareerItem);
            }

            tabCareers.Items.Add(new TabTextItem(" ", " ") {
                CanBeFocused = false
            });

            TabTextItem newCareer = new TabTextItem("Start a New Career", "New Career", "Press enter to start a new San Andreas Patrol career.");
            newCareer.Activated += (s, e) => GameFiber.StartNew(CareerCreation.Start);
            tabCareers.Items.Add(newCareer);



            tabAgencies.Heists.Clear();

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

                tabAgencies.Heists.Add(new MissionInformation(agency.Name, agency.Description, information) {
                    Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + agency.Images["card"]))
                });

                foreach (AgencyStation station in agency.Stations) {
                    MissionInformation stationMission = new MissionInformation(
                        "\t" + station.Name,
                        station.Description,

                        new Tuple<string, string>[] {
                            new Tuple<string, string>("Type", station.Type)
                        }
                    ) {
                        Logo = new MissionLogo(Game.CreateTextureFromFile("plugins/San Andreas Patrol/" + station.Images["card"]))
                    };

                    tabAgencies.Heists.Add(stationMission);
                }
            }


            TabView.Visible = true;
        }

        public static void Toggle() {
            if (TabView.Visible)
                Stop();
            else
                Start();
        }

        public static void Stop() {
            if (TabView.Visible == false)
                return;

            TabView.Visible = true;
        }
    }
}
