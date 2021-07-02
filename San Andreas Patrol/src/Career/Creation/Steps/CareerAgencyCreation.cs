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
using SanAndreasPatrol.Agencies.Stations;

namespace SanAndreasPatrol.Career.Creation.Steps {
    class CareerAgencyCreation {
        public static UIMenuListScrollerItem<string> Agencies;
        public static UIMenuListScrollerItem<string> Stations;
        public static UIMenuListScrollerItem<string> Difficulties;
        public static UIMenuItem Submit;

        public static void Start() {
            CareerCreation.Career.Agency = AgencyManager.GetDefaultAgency();
            CareerCreation.Career.Station = CareerCreation.Career.Agency.GetDefaultStation();
            CareerCreation.Career.Rank = CareerCreation.Career.Agency.Ranks.Find(x => x.Default);

            Agencies = new UIMenuListScrollerItem<string>("Agency", "", AgencyManager.GetAgencyAbbreviations());
            Stations = new UIMenuListScrollerItem<string>("Station", "", CareerCreation.Career.Agency.GetStationNames());
            Difficulties = new UIMenuListScrollerItem<string>("Difficulty", "", new string[] { "Normal", "Realistic" });
            Submit = new UIMenuItem("Continue", "Continue to the character creation.");

            Agencies.IndexChanged += OnAgencyChanged;
            Stations.IndexChanged += OnStationChanged;
            Difficulties.IndexChanged += OnDifficultyChanged;

            Submit.Activated += OnMenuSubmit;

            Agencies.Index = Agencies.Items.IndexOf(CareerCreation.Career.Agency.Abbreviation);
            Stations.Index = Stations.Items.IndexOf(CareerCreation.Career.Station.Name);

            CareerCreation.Menu.Visible = true;
            CareerCreation.Menu.SubtitleText = "Create a new career";
            CareerCreation.Menu.OnMenuClose += OnMenuCancel;
            CareerCreation.Menu.AddItems(Agencies, Stations, new UIMenuItem("") { Enabled = false }, Difficulties, new UIMenuItem("") { Enabled = false }, Submit);

            UpdateCamera();
        }

        public static void Stop() {
            CareerCreation.Menu.Visible = false;
            CareerCreation.Menu.SubtitleText = "";
            CareerCreation.Menu.OnMenuClose -= OnMenuCancel;
            CareerCreation.Menu.Clear();
        }

        public static void UpdateCamera() {
            AgencyStationCamera agencyStationCamera = CareerCreation.Career.Station.Cameras.FirstOrDefault(x => x.Type == "card");

            Game.LocalPlayer.Character.Position = CareerCreation.Camera.Position = agencyStationCamera.Position;
            CareerCreation.Camera.Rotation = agencyStationCamera.Rotation;
        }

        public static void OnAgencyChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            CareerCreation.Career.Agency = AgencyManager.GetAgencyByAbbreviation(Agencies.Items[newIndex]);
            CareerCreation.Career.Station = CareerCreation.Career.Agency.GetDefaultStation();
            CareerCreation.Career.Rank = CareerCreation.Career.Agency.Ranks.Find(x => x.Default);

            Stations.Items.Clear();

            Stations.Items = CareerCreation.Career.Agency.GetStationNames();
            Stations.Index = Stations.Items.IndexOf(CareerCreation.Career.Station.Name);

            UpdateCamera();
        }

        public static void OnStationChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            CareerCreation.Career.Station = CareerCreation.Career.Agency.GetStationByName(Stations.Items[newIndex]);

            UpdateCamera();
        }

        public static void OnDifficultyChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            if (Difficulties.Items[newIndex] == "Realistic")
                Difficulties.Description = "Your career will end if you die in the line of duty.";
            else
                Difficulties.Description = "";
        }

        public static void OnMenuSubmit(UIMenu sender, UIMenuItem selectedItem) {
            CareerCreation.Career.Difficulty = (Difficulties.SelectedItem == "Normal")?(CareerDifficulty.Normal):(CareerDifficulty.Realistic);

            GameFiber.StartNew(() => {
                Game.FadeScreenOut(1000, true);

                Stop();

                CareerCharacterCreation.Start();

                Game.FadeScreenIn(1000);
            });
        }

        public static void OnMenuCancel(UIMenu sender) {
            CareerCreation.Dispose();

            Stop();
        }
    }
}
