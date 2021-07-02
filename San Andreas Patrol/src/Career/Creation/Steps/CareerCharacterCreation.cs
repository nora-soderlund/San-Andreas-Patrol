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

        public static UIMenuItem Parents;
        public static UIMenuItem Name;
        public static UIMenuItem Items;

        public static UIMenuItem Submit;

        public static void Start() {
            CareerCreation.Career.Firstname = CareerManager.Names["male"][new Random().Next(CareerManager.Names["male"].Count)];
            CareerCreation.Career.Lastname = CareerManager.Names["family"][new Random().Next(CareerManager.Names["family"].Count)];

            Gender = new UIMenuListScrollerItem<string>("Sex", "Changing the sex will reset your changes.", new string[] { "Male", "Female" });
            
            Parents = new UIMenuItem("Parents", "Press enter to change your parents.") {
                RightLabel = "Default"
            };

            Name = new UIMenuItem("Name", "Press enter to change your name.") {
                RightLabel = CareerCreation.Career.Firstname + " " + CareerCreation.Career.Lastname
            };

            Name.Activated += (s, e) => GameFiber.StartNew(() => {
                CareerCreation.Menu.Visible = false;

                Game.DisplaySubtitle("Enter your character's name...");

                NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "FMMC_KEY_TIP", "p2", CareerCreation.Career.Firstname + " " + CareerCreation.Career.Lastname, "", "", "", 205);
            
                while(true) {
                    GameFiber.Yield();

                    int status = NativeFunction.CallByName<int>("UPDATE_ONSCREEN_KEYBOARD");

                    if (status == 1) {
                        string name = (string)NativeFunction.CallByName("GET_ONSCREEN_KEYBOARD_RESULT", typeof(string));

                        int breaker = name.LastIndexOf(' ');

                        if (breaker == -1)
                            break;

                        CareerCreation.Career.Firstname = name.Substring(0, breaker);
                        CareerCreation.Career.Lastname = name.Substring(breaker + 1);

                        break;
                    }
                    else if (status == 2)
                        break;
                }

                CareerCreation.Menu.Visible = true;

                Name.RightLabel = CareerCreation.Career.Firstname + " " + CareerCreation.Career.Lastname;
            });

            Items = new UIMenuItem("Hair, ears, and glasses..", "Press enter to change your hair and glasses.");

            Submit = new UIMenuItem("Submit", "Submit the character creation.");

            Gender.IndexChanged += OnGenderChanged;
            Parents.Activated += OnParentsClick;
            Items.Activated += OnItemsClick;


            Submit.Activated += OnMenuSubmit;

            OnGenderChanged(Gender, Gender.Index, Gender.Index);

            CareerCreation.Menu.Visible = true;
            CareerCreation.Menu.SubtitleText = "Customize your character";
            CareerCreation.Menu.OnMenuClose += OnMenuCancel;
            CareerCreation.Menu.AddItems(Gender, Parents, Name, new UIMenuItem("") { Enabled = false }, Items, new UIMenuItem("") { Enabled = false }, Submit);

            UpdateCamera();
        }

        private static void OnParentsClick(UIMenu clickSender, UIMenuItem selectedItem) {
            CareerCreation.Menu.Visible = false;

            UIMenu menu = new UIMenu("Career", "Customize your character's parents") {
                Visible = true
            };

            UIMenuListScrollerItem<string> sexuality = new UIMenuListScrollerItem<string>("Sexuality", "", new string[] {
                "Heterosexual", "Homosexual (Gay)", "Homosexual (Lesbian)"
            });
            sexuality.Index = 0;

            UIMenuListScrollerItem<string> parent1 = new UIMenuListScrollerItem<string>("Parent #1", "", HeadBlend.Fathers.Select(x => x.Value).ToList());
            parent1.Index = 0;

            UIMenuListScrollerItem<string> parent2 = new UIMenuListScrollerItem<string>("Parent #2", "", HeadBlend.Mothers.Select(x => x.Value).ToList());
            parent2.Index = 0;

            UIMenuNumericScrollerItem<float> headMixture = new UIMenuNumericScrollerItem<float>("Head Mixture", "", 0.0f, 1.0f, 0.1f) {
                SliderBar = new UIMenuScrollerSliderBar() {
                    Width = 0.5f,
                    Height = 0.4f,

                    Markers = {
                        new UIMenuScrollerSliderBarMarker(0.5f)
                    }
                },

                AllowWrapAround = false,
                Value = 0.05f
            };

            UIMenuNumericScrollerItem<float> skinMixture = new UIMenuNumericScrollerItem<float>("Skin Mixture", "", 0.0f, 1.0f, 0.1f) {
                SliderBar = new UIMenuScrollerSliderBar() {
                    Width = 0.5f,
                    Height = 0.4f,

                    Markers = {
                        new UIMenuScrollerSliderBarMarker(0.5f)
                    }
                },

                AllowWrapAround = false,
                Value = 0.05f
            };

            ItemScrollerEvent onParentChanged = (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                if (sexuality.Index == 0) {
                    KeyValuePair<int, string> parent1Pair = HeadBlend.Fathers.Where(x => x.Value == parent1.SelectedItem).FirstOrDefault();
                    KeyValuePair<int, string> parent2Pair = HeadBlend.Mothers.Where(x => x.Value == parent2.SelectedItem).FirstOrDefault();

                    CareerCreation.Career.Character.Parent1 = parent1Pair.Key;
                    CareerCreation.Career.Character.Parent2 = parent2Pair.Key;

                    Parents.RightLabel = parent1Pair.Value + " & " + parent2Pair.Value;
                }
                else if (sexuality.Index == 1) {
                    KeyValuePair<int, string> parent1Pair = HeadBlend.Fathers.Where(x => x.Value == parent1.SelectedItem).FirstOrDefault();
                    KeyValuePair<int, string> parent2Pair = HeadBlend.Fathers.Where(x => x.Value == parent2.SelectedItem).FirstOrDefault();

                    CareerCreation.Career.Character.Parent1 = parent1Pair.Key;
                    CareerCreation.Career.Character.Parent2 = parent2Pair.Key;

                    Parents.RightLabel = parent1Pair.Value + " & " + parent2Pair.Value;
                }
                else if (sexuality.Index == 2) {
                    KeyValuePair<int, string> parent1Pair = HeadBlend.Mothers.Where(x => x.Value == parent1.SelectedItem).FirstOrDefault();
                    KeyValuePair<int, string> parent2Pair = HeadBlend.Mothers.Where(x => x.Value == parent2.SelectedItem).FirstOrDefault();

                    CareerCreation.Career.Character.Parent1 = parent1Pair.Key;
                    CareerCreation.Career.Character.Parent2 = parent2Pair.Key;

                    Parents.RightLabel = parent1Pair.Value + " & " + parent2Pair.Value;
                }

                CareerCreation.Career.Character.Update();
            };

            sexuality.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                parent1.Items.Clear();
                parent2.Items.Clear();

                if (newIndex == 0) {
                    parent1.Items = HeadBlend.Fathers.Select(x => x.Value).ToList();
                    parent2.Items = HeadBlend.Mothers.Select(x => x.Value).ToList();
                }
                else if (newIndex == 1) {
                    parent1.Items = HeadBlend.Fathers.Select(x => x.Value).ToList();
                    parent2.Items = HeadBlend.Fathers.Select(x => x.Value).ToList();
                }
                else if (newIndex == 2) {
                    parent1.Items = HeadBlend.Mothers.Select(x => x.Value).ToList();
                    parent2.Items = HeadBlend.Mothers.Select(x => x.Value).ToList();
                }

                if (parent1.Index > parent1.OptionCount)
                    parent1.Index = 0;

                if (parent2.Index > parent2.OptionCount)
                    parent2.Index = 0;

                onParentChanged(null, 0, 0);
            };

            parent1.IndexChanged += onParentChanged;
            parent2.IndexChanged += onParentChanged;

            headMixture.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.HeadMixture = headMixture.Value;

                CareerCreation.Career.Character.Update();
            };

            skinMixture.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.SkinMixture = skinMixture.Value;

                CareerCreation.Career.Character.Update();
            };

            menu.AddItems(sexuality, new UIMenuItem("") { Enabled = false }, parent1, parent2, new UIMenuItem("") { Enabled = false }, headMixture, skinMixture);

            menu.OnMenuClose += (UIMenu sender) => {
                menu.Visible = false;

                EntryPoint.MenuPool.Remove(menu);

                CareerCreation.Menu.Visible = true;
            };

            EntryPoint.MenuPool.Add(menu);
        }

        private static void OnItemsClick(UIMenu clickSender, UIMenuItem selectedItem) {
            CareerCreation.Menu.Visible = false;

            UIMenu menu = new UIMenu("Career", "Customize your character's items") {
                Visible = true
            };

            UIMenuNumericScrollerItem<int> hair = new UIMenuNumericScrollerItem<int>("Hair", "", 0, Game.LocalPlayer.Character.GetDrawableVariationCount(2), 1);
            UIMenuNumericScrollerItem<int> hairColor = new UIMenuNumericScrollerItem<int>("Hair Color", "", 0, NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 2, CareerCreation.Career.Character.Hair), 1);

            hair.Index = (CareerCreation.Career.Character.Hair != 255)?(CareerCreation.Career.Character.Hair):(0);
            hairColor.Index = CareerCreation.Career.Character.HairColor;

            hair.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.Hair = newIndex;
                CareerCreation.Career.Character.Update();

                hairColor.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 2, CareerCreation.Career.Character.Hair);

                if (hairColor.Index > hairColor.Maximum)
                    hairColor.Index = 0;
            };

            hairColor.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.HairColor = newIndex;
                CareerCreation.Career.Character.Update();
            };

            UIMenuNumericScrollerItem<int> ears = new UIMenuNumericScrollerItem<int>("Ears", "", 0, NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", Game.LocalPlayer.Character, 2), 1);
            UIMenuNumericScrollerItem<int> earsTexture = new UIMenuNumericScrollerItem<int>("Ears Texture", "", 0, NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 2, CareerCreation.Career.Character.Glasses), 1);

            ears.Index = (CareerCreation.Career.Character.Ears != 255) ? (CareerCreation.Career.Character.Ears) : (0);
            earsTexture.Index = CareerCreation.Career.Character.EarsTexture;

            ears.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.Ears = (newIndex == 0)?(255):(newIndex);
                CareerCreation.Career.Character.Update();

                earsTexture.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 2, CareerCreation.Career.Character.Ears);

                if (earsTexture.Index > earsTexture.Maximum)
                    earsTexture.Index = 0;
            };

            earsTexture.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.EarsTexture = newIndex;
                CareerCreation.Career.Character.Update();
            };

            UIMenuNumericScrollerItem<int> glasses = new UIMenuNumericScrollerItem<int>("Glasses", "", 0, NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_DRAWABLE_VARIATIONS", Game.LocalPlayer.Character, 1), 1);
            UIMenuNumericScrollerItem<int> glassesTexture = new UIMenuNumericScrollerItem<int>("Glasses Texture", "", 0, NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 1, CareerCreation.Career.Character.Glasses), 1);

            glasses.Index = (CareerCreation.Career.Character.Glasses != 255) ? (CareerCreation.Career.Character.Glasses) : (0);
            glassesTexture.Index = CareerCreation.Career.Character.GlassesTexture;

            glasses.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.Glasses = newIndex;
                CareerCreation.Career.Character.Update();

                glassesTexture.Maximum = NativeFunction.CallByName<int>("GET_NUMBER_OF_PED_PROP_TEXTURE_VARIATIONS", Game.LocalPlayer.Character, 1, CareerCreation.Career.Character.Glasses);

                if (glassesTexture.Index > glassesTexture.Maximum)
                    glassesTexture.Index = 0;
            };

            glassesTexture.IndexChanged += (UIMenuScrollerItem sender, int oldIndex, int newIndex) => {
                CareerCreation.Career.Character.GlassesTexture = newIndex;
                CareerCreation.Career.Character.Update();
            };

            menu.AddItems(hair, hairColor, new UIMenuItem("") { Enabled = false }, ears, earsTexture, new UIMenuItem("") { Enabled = false }, glasses, glassesTexture);

            EntryPoint.MenuPool.Add(menu);

            menu.OnMenuClose += (UIMenu sender) => {
                menu.Visible = false;

                EntryPoint.MenuPool.Remove(menu);

                CareerCreation.Menu.Visible = true;
            };
        }

        private static void OnMenuSubmit(UIMenu sender, UIMenuItem selectedItem) {
            GameFiber.StartNew(() => {
                Game.FadeScreenOut(1000, true);

                Stop();

                CareerCreation.Finish();
            });
        }

        public static void OnMenuCancel(UIMenu sender) {
            GameFiber.StartNew(() => {
                Game.FadeScreenOut(1000, true);

                Stop();

                CareerAgencyCreation.Start();

                Game.FadeScreenIn(1000);
            });
        }

        public static void Stop() {
            CareerCreation.Menu.Visible = false;
            CareerCreation.Menu.SubtitleText = "";
            CareerCreation.Menu.OnMenuClose -= OnMenuCancel;
            CareerCreation.Menu.Clear();
        }

        private static void OnGenderChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex) {
            CareerCreation.Career.Character = new CareerCharacter();
            CareerCreation.Career.Character.Gender = Data.Genders[Gender.SelectedItem.ToLower()];
            CareerCreation.Career.Character.Apply();

            AgencyOutfit agencyOutfit = CareerCreation.Career.Agency.Outfits.Find(x => x.Type == AgencyOutfitType.Formal && x.Gender == CareerCreation.Career.Character.Gender);
            agencyOutfit.Apply(Game.LocalPlayer.Character);

            CareerCreation.Career.Firstname = CareerManager.Names[Gender.SelectedItem.ToLower()][new Random().Next(CareerManager.Names[Gender.SelectedItem.ToLower()].Count)];

            Parents.RightLabel = "Default";
            Name.RightLabel = CareerCreation.Career.Firstname + " " + CareerCreation.Career.Lastname;
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
