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

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Agencies.Stations;

namespace SanAndreasPatrol.Career {
    class Career {
        public string Id;

        public string Firstname;
        public string Lastname;

        public Agency Agency;
        public AgencyStation Station;
        public AgencyRank Rank;

        public CareerDifficulty Difficulty;

        public CareerCharacter Character = new CareerCharacter();
        public List<CareerVehicle> Vehicles = new List<CareerVehicle>();

        public Career() {
            string id = Guid.NewGuid().ToString();

            while(File.Exists("plugins/San Andreas Patrol/careers/" + id + ".xml") || CareerManager.Careers.Count(x => x.Id == id) != 0)
                id = Guid.NewGuid().ToString();

            Id = id;
        }

        public Career(string id) {
            Id = id;

            EntryPoint.Print(id);

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/careers/" + Id + ".xml");

            XElement xCareer = xDocument.Element("Career");

            Firstname = xCareer.Element("Firstname").Value;
            Lastname = xCareer.Element("Lastname").Value;

            Agency = AgencyManager.GetAgencyById(xCareer.Element("Agency").Value);
            Station = Agency.GetStationById(xCareer.Element("Station").Value);
            Rank = Agency.Ranks.Find(x => x.Id == int.Parse(xCareer.Element("Rank").Value));

            Difficulty = (CareerDifficulty)int.Parse(xCareer.Element("Difficulty").Value);

            XElement xCharacter = xCareer.Element("Character");

            Character.Gender = (Gender)int.Parse(xCharacter.Element("Gender").Value);

            XElement xParents = xCharacter.Element("Parents");

            Character.Parent1 = int.Parse(xParents.Attribute("parent1").Value);
            Character.Parent2 = int.Parse(xParents.Attribute("parent2").Value);

            Character.HeadMixture = float.Parse(xParents.Attribute("head").Value);
            Character.SkinMixture = float.Parse(xParents.Attribute("skin").Value);

            XElement xHair = xCharacter.Element("Hair");

            Character.Hair = int.Parse(xHair.Attribute("drawable").Value);
            Character.HairColor = int.Parse(xHair.Attribute("color").Value);

            XElement xEars = xCharacter.Element("Ears");

            Character.Ears = int.Parse(xEars.Attribute("drawable").Value);
            Character.EarsTexture = int.Parse(xEars.Attribute("texture").Value);

            XElement xGlasses = xCharacter.Element("Glasses");

            Character.Glasses = int.Parse(xGlasses.Attribute("drawable").Value);
            Character.GlassesTexture = int.Parse(xGlasses.Attribute("texture").Value);

            XElement xVehicles = xCharacter.Element("Vehicles");

            if (xVehicles != null && xVehicles.Value != null) {
                foreach (XElement xVehicle in xVehicles.Elements("Vehicle")) {
                    XElement xVehicleSpawn = xVehicle.Element("Spawn");

                    XElement xPosition = xVehicleSpawn.Element("Position");
                    XElement xRotation = xVehicleSpawn.Element("Rotation");

                    CareerVehicle careerVehicle = new CareerVehicle() {
                        Model = xVehicle.Element("Model").Value,

                        Position = new Vector3(
                            float.Parse(xPosition.Attribute("breadth").Value),
                            float.Parse(xPosition.Attribute("height").Value),
                            float.Parse(xPosition.Attribute("depth").Value)),

                        Rotation = new Rotator(
                            float.Parse(xRotation.Attribute("breadth").Value),
                            float.Parse(xRotation.Attribute("height").Value),
                            float.Parse(xRotation.Attribute("depth").Value))
                    };

                    Vehicles.Add(careerVehicle);
                }
            }
        }

        public void Save() {
            Game.DisplayHelp($"~{InstructionalKey.SymbolBusySpinner.GetId()}~ Saving career...", true);

            XDocument xDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),

                new XElement("Career",
                    new XAttribute("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds()),

                    new XElement("Firstname", Firstname),
                    new XElement("Lastname", Lastname),

                    new XElement("Agency", Agency.Id),
                    new XElement("Station", Station.Id),
                    new XElement("Rank", Rank.Id),

                    new XElement("Difficulty", (int)Difficulty),

                    new XElement("Character",
                        new XElement("Gender", (int)Character.Gender),

                        new XElement("Parents",
                            new XAttribute("parent1", Character.Parent1),
                            new XAttribute("parent2", Character.Parent2),

                            new XAttribute("head", Character.HeadMixture),
                            new XAttribute("skin", Character.SkinMixture)
                        ),

                        new XElement("Hair",
                            new XAttribute("drawable", Character.Hair),
                            new XAttribute("color", Character.HairColor)
                        ),

                        new XElement("Ears",
                            new XAttribute("drawable", Character.Ears),
                            new XAttribute("texture", Character.EarsTexture)
                        ),

                        new XElement("Glasses",
                            new XAttribute("drawable", Character.Glasses),
                            new XAttribute("texture", Character.GlassesTexture)
                        )
                    )
                )
            );

            xDocument.Save("plugins/San Andreas Patrol/careers/" + Id + ".xml");

            Game.HideHelp();
        }
    }

    enum CareerDifficulty {
        Normal = 0, Realistic
    }

    class CareerCharacter {
        public Gender Gender = Gender.Unisex;

        public int Parent1 = 0;
        public int Parent2 = 0;

        public float HeadMixture = 0.5f;
        public float SkinMixture = 0.5f;

        public int Hair = 255;
        public int HairColor;

        public int Ears = 255;
        public int EarsTexture;

        public int Glasses = 255;
        public int GlassesTexture;

        public void Apply() {
            Game.LocalPlayer.Model = new Model((Gender == Gender.Male) ? ("mp_m_freemode_01") : ("mp_f_freemode_01"));

            Game.LocalPlayer.Character.ResetVariation();

            Update();
        }

        public void Update() {
            HeadBlend.SetDataForPed(Game.LocalPlayer.Character, new HeadBlendData() {
                shapeFirstID = Parent1,
                shapeSecondID = Parent2,
                
                skinFirstID = Parent1,
                skinSecondID = Parent2,

                shapeMix = HeadMixture,
                skinMix = SkinMixture
            });

            Game.LocalPlayer.Character.SetVariation(2, Hair, 0);
            HeadBlend.SetHairColor(Game.LocalPlayer.Character, HairColor, HairColor);

            NativeFunction.CallByName<int>("SET_PED_PROP_INDEX", Game.LocalPlayer.Character, 1, Glasses, GlassesTexture, true);

            NativeFunction.CallByName<int>("SET_PED_PROP_INDEX", Game.LocalPlayer.Character, 2, Ears, EarsTexture, true);
        }
    }

    class CareerVehicle {
        public string Model;

        public Vector3 Position;
        public Rotator Rotation;
    }
}
