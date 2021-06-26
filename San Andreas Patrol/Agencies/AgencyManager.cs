using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using SanAndreasPatrol.Agencies.Stations;

namespace SanAndreasPatrol.Agencies {
    class AgencyManager {
        public static List<Agency> Agencies = new List<Agency>();

        public static void Main() {
            Game.Console.Print("[San Andreas Patrol] Reading agencies.xml from \"plugins/San Andreas Patrol/agencies.xml\"...");

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/agencies.xml");

            XElement xAgencies = xDocument.Element("Agencies");

            foreach(XElement xAgency in xAgencies.Elements("Agency")) {
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
                            Gender = Data.Genders[outfit.Attribute("gender").Value]
                        };

                        foreach (XElement part in outfit.Elements("Part")) {
                            AgencyOutfitPart agencyOutfitPart = new AgencyOutfitPart() {
                                Id = part.Attribute("id").Value,
                                Drawable = int.Parse(part.Attribute("drawable").Value) - 1,
                                Texture = int.Parse(part.Attribute("texture").Value) - 1
                            };

                            agencyOutfit.Parts.Add(agencyOutfitPart);
                        }

                        agency.Outfits.Add(agencyOutfit);
                    }
                }

                if (!xAgency.Element("Stations").IsEmpty) {
                    foreach (XElement xStation in xAgency.Element("Stations").Elements("Station")) {
                        Game.Console.Print("[San Andreas Patrol] Parsing station id:" + xStation.Attribute("id").Value + " " + xStation.Element("Name").Value + "...");

                        AgencyStation station = new AgencyStation() {
                            Id = xStation.Attribute("id").Value,

                            Default = (xStation.Attribute("default") != null),
                            Disabled = (xStation.Attribute("disabled") != null),

                            Name = xStation.Element("Name").Value,
                            Description = (!xStation.Element("Description").IsEmpty) ? (xStation.Element("Description").Value) : (""),

                            Type = (!xStation.Element("Type").IsEmpty) ? (xStation.Element("Type").Value) : ("")
                        };

                        if (!xStation.Element("Camera").IsEmpty) {
                            XElement xCamera = xStation.Element("Camera");

                            XElement xPosition = xCamera.Element("Position");

                            station.CameraPosition = new Vector3(
                                float.Parse(xPosition.Attribute("breadth").Value),
                                float.Parse(xPosition.Attribute("height").Value),
                                float.Parse(xPosition.Attribute("depth").Value));

                            XElement xRotation = xCamera.Element("Rotation");

                            station.CameraRotation = new Rotator(
                                float.Parse(xRotation.Attribute("breadth").Value),
                                float.Parse(xRotation.Attribute("height").Value),
                                float.Parse(xRotation.Attribute("depth").Value));
                        }

                        if (!xStation.Element("Images").IsEmpty) {
                            foreach (XElement image in xStation.Element("Images").Elements("Image")) {
                                station.Images.Add(image.Attribute("id").Value, image.Value);
                            }
                        }

                        agency.Stations.Add(station);
                    }
                }

                if (!xAgency.Element("Images").IsEmpty) {
                    foreach (XElement image in xAgency.Element("Images").Elements("Image")) {
                        agency.Images.Add(image.Attribute("id").Value, image.Value);
                    }
                }

                Agencies.Add(agency);

                Game.Console.Print("[San Andreas Patrol] Loaded agency id:" + xAgency.Attribute("id").Value + " " + xAgency.Element("Name").Value + " with " + agency.Stations.Count + " stations...");
            }
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
