using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using SanAndreasPatrol.Data;

namespace SanAndreasPatrol.Agencies {
    class AgencyManager {
        public static List<Agency> Agencies = new List<Agency>();

        public static void Start() {
            Game.Console.Print("[San Andreas Patrol] Reading agencies.xml from \"plugins/San Andreas Patrol/agencies.xml\"...");

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/agencies.xml");

            XElement xAgencies = xDocument.Element("Agencies");

            foreach(XElement xAgency in xAgencies.Elements("Agency")) {
                Game.Console.Print("[San Andreas Patrol] Parsing agency id:" + xAgency.Attribute("id").Value + " " + xAgency.Element("Name").Value + "...");

                Agency agency = new Agency() {
                    Id = xAgency.Attribute("id").Value,
                    Default = (xAgency.Attribute("default") != null),
                    Disabled = (xAgency.Attribute("disabled") != null),

                    Name = xAgency.Element("Name").Value,
                    Description = (!xAgency.Element("Description").IsEmpty)?(xAgency.Element("Description").Value):(""),

                    Motto = (!xAgency.Element("Motto").IsEmpty) ? (xAgency.Element("Motto").Value) : (""),
                    Abbreviation = xAgency.Element("Abbreviation").Value,

                    Formed = xAgency.Element("Formed").Value,
                };

                if (!xAgency.Element("Employees").IsEmpty) {
                    agency.SwornEmployees = int.Parse(xAgency.Element("Employees").Element("Sworn").Value);
                    agency.UnswornEmployees = int.Parse(xAgency.Element("Employees").Element("Unsworn").Value);
                }

                if (!xAgency.Element("Ranks").IsEmpty) {
                    foreach (XElement rank in xAgency.Element("Ranks").Elements("Rank")) {
                        agency.Ranks.Add(rank.Value);
                    }
                }

                if (!xAgency.Element("Outfits").IsEmpty) {
                    foreach (XElement outfit in xAgency.Element("Outfits").Elements("Outfit")) {
                        AgencyOutfit agencyOutfit = new AgencyOutfit() {
                            Id = outfit.Attribute("id").Value,
                            Type = AgencyOutfit.Types[outfit.Attribute("type").Value],
                            Gender = Data.Data.Genders[outfit.Attribute("gender").Value]
                        };

                        Game.Console.Print("outfit " + agencyOutfit.Id + " gender " + agencyOutfit.Gender.ToString() + " type " + agencyOutfit.Type.ToString());

                        foreach (XElement part in outfit.Elements("Part")) {
                            AgencyOutfitPart agencyOutfitPart = new AgencyOutfitPart() {
                                Id = part.Attribute("id").Value,
                                Drawable = int.Parse(part.Attribute("drawable").Value) - 1,
                                Texture = int.Parse(part.Attribute("texture").Value) - 1
                            };

                            Game.Console.Print("id " + agencyOutfitPart.Id + " drawable " + agencyOutfitPart.Drawable + " texture " + agencyOutfitPart.Texture);

                            agencyOutfit.Parts.Add(agencyOutfitPart);
                        }

                        agency.Outfits.Add(agencyOutfit);
                    }
                }

                if (!xAgency.Element("Images").IsEmpty) {
                    foreach (XElement image in xAgency.Element("Images").Elements("Image")) {
                        agency.Images.Add(image.Attribute("id").Value, image.Value);
                    }
                }

                Agencies.Add(agency);
            }

            Game.Console.Print("[San Andreas Patrol] Finished loading " + Agencies.Count + " agencies to memory!");
        }

        public static Agency GetDefaultAgency() {
            return Agencies.Find(x => x.Default) ?? Agencies.FirstOrDefault();
        }

        public static Agency GetAgencyById(string id) {
            return Agencies.Find(x => x.Id == id);
        }

        public static Agency GetAgencyByName(string name) {
            return Agencies.Find(x => x.Name == name);
        }

        public static Agency GetAgencyByAbbreviation(string abbreviation) {
            return Agencies.Find(x => x.Abbreviation == abbreviation);
        }

        public static List<string> GetAgencyNames() {
            List<string> names = new List<string>();

            foreach (Agency agency in Agencies.Where(x => x.Disabled != true))
                names.Add(agency.Name);

            return names;
        }

        public static List<string> GetAgencyAbbreviations() {
            List<string> abbreviations = new List<string>();

            foreach (Agency agency in Agencies.Where(x => x.Disabled != true))
                abbreviations.Add(agency.Abbreviation);

            return abbreviations;
        }
    }
}
