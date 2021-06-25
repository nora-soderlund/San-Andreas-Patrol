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

namespace SanAndreasPatrol.Career.Creation {
    class CareerAgencyCreation {
        public static UIMenu Menu;

        public static UIMenuListScrollerItem<string> Agencies;
        public static UIMenuListScrollerItem<string> Stations;
        public static UIMenuListScrollerItem<string> Difficulties;
        public static UIMenuItem Submit;

        public static Agency Agency;
        public static Station Station;

        public static void Start() {
            Menu = new UIMenu("Career", "Create a new career") {
                Visible = true
            };

            Menu.OnMenuClose += OnMenuClose;

            Agency = AgencyManager.GetDefaultAgency();
            Station = StationManager.GetDefaultStation(Agency.Id);

            Agencies = new UIMenuListScrollerItem<string>("Agency", "", AgencyManager.GetAgencyAbbreviations());
            Stations = new UIMenuListScrollerItem<string>("Station", "", StationManager.GetStationNamesByAgency(Agency.Id));
            Difficulties = new UIMenuListScrollerItem<string>("Difficulty", "", new string[] { "Normal", "Realistic" });
            Submit = new UIMenuItem("Continue", "Continue to the character creation.");

            Agencies.IndexChanged += OnAgencyChanged;
            Stations.IndexChanged += OnStationChanged;
            Difficulties.IndexChanged += OnDifficultyChanged;
            Submit.Activated += OnMenuFinish;

            Agencies.Index = Agencies.Items.IndexOf(Agency.Abbreviation);
            Stations.Index = Stations.Items.IndexOf(Station.Name);

            Menu.AddItems(Agencies, Stations, new UIMenuItem("") { Enabled = false }, Difficulties, new UIMenuItem("") { Enabled = false }, Submit);

            EntryPoint.MenuPool.Add(Menu);

            UpdateCamera();
        }

        public static void Dispose() {
            Game.Console.Print("CareerCreationAgency.Dispose");

            Menu.Visible = false;

            EntryPoint.MenuPool.Remove(Menu);
        }

        public static void UpdateCamera() {
            Game.LocalPlayer.Character.Position = CareerCreation.Camera.Position = new Vector3(Station.CameraPosition.X, Station.CameraPosition.Y, Station.CameraPosition.Z);
            CareerCreation.Camera.Rotation = new Rotator(Station.CameraRotation.Pitch, Station.CameraRotation.Roll, Station.CameraRotation.Yaw);
        }

        public static void OnAgencyChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            Agency = AgencyManager.GetAgencyByAbbreviation(Agencies.Items[newIndex]);
            Station = StationManager.GetDefaultStation(Agency.Id);

            Stations.Items.Clear();

            Stations.Items = StationManager.GetStationNamesByAgency(Agency.Id);
            Stations.Index = Stations.Items.IndexOf(Station.Name);

            UpdateCamera();
        }

        public static void OnStationChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            Station = StationManager.GetStationByName(Stations.Items[newIndex]);

            UpdateCamera();
        }

        public static void OnDifficultyChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            if (Difficulties.Items[newIndex] == "Realistic")
                Difficulties.Description = "Your career will end if you die in the line of duty.";
            else
                Difficulties.Description = "";
        }

        public static void OnMenuFinish(UIMenu sender, UIMenuItem selectedItem) {
            Game.Console.Print("OnMenuFinish");

            CareerCreation.Agency = Agency;
            CareerCreation.Station = Station;
            CareerCreation.Difficulty = Difficulties.SelectedItem;

            Game.FadeScreenOut(1000, true);

            Dispose();

            CareerCharacterCreation.Start();

            Game.FadeScreenIn(1000);
        }

        public static void OnMenuClose(UIMenu sender) {
            Game.Console.Print("OnMenuClose");

            CareerCreation.Dispose();

            Dispose();
        }
    }
}
