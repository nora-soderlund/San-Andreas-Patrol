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
    class CareerCharacterCreation {
        public static UIMenuListScrollerItem<string> Gender;

        public static UIMenuNumericScrollerItem<int> Hair;
        public static UIMenuNumericScrollerItem<int> Glasses;
        public static UIMenuNumericScrollerItem<int> GlassesTexture;

        public static UIMenuItem Submit;

        public static Dictionary<string, string> GenderModels = new Dictionary<string, string>() {
            { "Male", "mp_m_freemode_01" },
            { "Female", "mp_f_freemode_01" }
        };

        public static void Start() {
            Gender = new UIMenuListScrollerItem<string>("Gender", "Changing the gender will reset your changes.", GenderModels.Keys);
            Hair = new UIMenuNumericScrollerItem<int>("Hair", "", 0, 0, 1);
            Glasses = new UIMenuNumericScrollerItem<int>("Glasses", "", 0, 0, 1);
            GlassesTexture = new UIMenuNumericScrollerItem<int>("Glasses Texture", "", 0, 0, 1);

            Submit = new UIMenuItem("Finish", "Finish the character creation.");

            Gender.IndexChanged += OnGenderChanged;
            Hair.IndexChanged += OnHairChanged;
            Glasses.IndexChanged += OnGlassesChanged;
            GlassesTexture.IndexChanged += OnGlassesTextureChanged;

            Submit.Activated += OnMenuSubmit;

            OnGenderChanged(Gender, Gender.Index, Gender.Index);

            CareerCreation.Menu.Visible = true;
            CareerCreation.Menu.SubtitleText = "Cutomize your character";
            CareerCreation.Menu.OnMenuClose += OnMenuCancel;
            CareerCreation.Menu.AddItems(Gender, new UIMenuItem("") { Enabled = false }, Hair, new UIMenuItem("") { Enabled = false }, Glasses, GlassesTexture, new UIMenuItem("") { Enabled = false }, Submit);

            UpdateCamera();
        }

        private static void OnMenuSubmit(UIMenu sender, UIMenuItem selectedItem) {
            CareerCreation.Career.Character.Hair = Hair.Index;

            CareerCreation.Career.Character.Glasses = Glasses.Index;
            CareerCreation.Career.Character.GlassesTexture = GlassesTexture.Index;

            GameFiber.StartNew(() => {
                Game.FadeScreenOut(1000, true);

                CareerCreation.Dispose();

                Stop();

                Game.FadeScreenIn(1000);
            });
        }

        public static void OnMenuCancel(UIMenu sender) {
            CareerCreation.Dispose();

            Stop();
        }

        public static void Stop() {
            CareerCreation.Menu.Visible = false;
            CareerCreation.Menu.SubtitleText = "";
            CareerCreation.Menu.OnMenuClose -= OnMenuCancel;
            CareerCreation.Menu.Clear();
        }

        private static void OnGenderChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            Game.LocalPlayer.Model = new Model(GenderModels[Gender.SelectedItem]);

            Game.LocalPlayer.Character.ResetVariation();

            AgencyOutfit agencyOutfit = CareerCreation.Career.Agency.Outfits.Find(x => x.Type == AgencyOutfitType.Formal && x.Gender == Data.Genders[Gender.SelectedItem.ToLower()]);

            foreach(AgencyOutfitPart agencyOutfitPart in agencyOutfit.Parts)
                Game.LocalPlayer.Character.SetVariation(EntryPoint.ComponentIndex[agencyOutfitPart.Id], agencyOutfitPart.Drawable, agencyOutfitPart.Texture);

            Hair.Maximum = Game.LocalPlayer.Character.GetDrawableVariationCount(2);
            Hair.Index = 0;

            /*foreach(ClothingPartData clothingPartData in Data.Data.Clothing.Find(x => x.Id == "hair" && x.Gender == ((Gender.SelectedItem == "Male") ? (ClothingGender.Male) : (ClothingGender.Female))).Parts)
                Hair.Items.Add(clothingPartData.Name);*/

            //Hair.Index = 0;

            Glasses.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", Game.LocalPlayer.Character, 1);
            Glasses.Index = 0;

            GlassesTexture.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 1, Glasses.Index) - 1;
            GlassesTexture.Index = 0;
            GlassesTexture.Enabled = (GlassesTexture.Maximum != 1);
        }

        private static void OnHairChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            //ClothingPartData clothingPartData = Data.Data.Clothing.Find(x => x.Id == "hair" && x.Gender == ((Gender.SelectedItem == "Male") ? (ClothingGender.Male) : (ClothingGender.Female))).Parts.Find(x => x.Name == Hair.SelectedItem
            
            Game.LocalPlayer.Character.SetVariation(2, newIndex, 0);
        }

        private static void OnGlassesChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            NativeFunction.CallByName<int>("SET_PED_PROP_INDEX", Game.LocalPlayer.Character, 1, newIndex, 0, true);

            GlassesTexture.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 1, newIndex) - 1;
            GlassesTexture.Index = 0;
            GlassesTexture.Enabled = (GlassesTexture.Maximum != 1);
        }

        private static void OnGlassesTextureChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            NativeFunction.CallByName<int>("SET_PED_PROP_INDEX", Game.LocalPlayer.Character, 1, Glasses.Index, newIndex, true);
        }

        public static void UpdateCamera() {
            Game.LocalPlayer.Character.Position = new Vector3(402.8664f, -996.4108f, -99.00027f);
            Game.LocalPlayer.Character.Heading = -185.0f;

            CareerCreation.Camera.Position = new Vector3(402.8664f, -997.5515f, -98.5f);
            CareerCreation.Camera.Rotation = new Rotator(0.0f, 0.0f, 0.0f);
            CareerCreation.Camera.Heading = 360.0f - -185.0f;

            Game.LocalPlayer.Character.IsVisible = true;
        }
    }
}
