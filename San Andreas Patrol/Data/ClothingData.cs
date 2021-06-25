using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Native;
using Rage.Attributes;

namespace SanAndreasPatrol.Data {
    enum ClothingGender {
        Unisex = 0,
        Male = 1,
        Female = 2
    }

    class Data {
        public static Dictionary<string, ClothingGender> Genders = new Dictionary<string, ClothingGender>() {
            { "unisex", ClothingGender.Unisex },
            { "male", ClothingGender.Male },
            { "female", ClothingGender.Female }
        };

        public static List<ClothingData> Clothing = new List<ClothingData>();

        public static void Start() {
            XElement xClothing = XDocument.Load("plugins/San Andreas Patrol/data/clothing.xml").Element("Clothing");

            foreach(XElement xComponent in xClothing.Elements("Component")) {
                ClothingData clothingData = new ClothingData() {
                    Id = xComponent.Attribute("id").Value,
                    Gender = Genders[xComponent.Attribute("gender").Value]
                };

                foreach(XElement xPart in xComponent.Elements("Part")) {
                    ClothingPartData clothingPartData = new ClothingPartData() {
                        Drawable = int.Parse(xPart.Attribute("drawable").Value) - 1,
                        Name = xPart.Attribute("name").Value
                    };

                    clothingData.Parts.Add(clothingPartData);
                }

                Clothing.Add(clothingData);
            }
        }
    }

    class ClothingData {
        public string Id;
        public ClothingGender Gender;

        public List<ClothingPartData> Parts = new List<ClothingPartData>();
    }

    class ClothingPartData {
        public int Drawable;
        public string Name;
    }
}
