using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol.Stations {
    class StationManager {
        public static List<Station> Stations = new List<Station>();

        public static void Start() {
            Game.Console.Print("[San Andreas Patrol] Reading stations.xml from \"plugins/San Andreas Patrol/stations.xml\"...");

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/stations.xml");

            XElement xStations = xDocument.Element("Stations");

            foreach (XElement xStation in xStations.Elements("Station")) {
                Game.Console.Print("[San Andreas Patrol] Parsing station id:" + xStation.Attribute("id").Value + " " + xStation.Element("Name").Value + "...");

                Station station = new Station() {
                    Id = xStation.Attribute("id").Value,
                    Agency = xStation.Attribute("agency").Value,

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

                Stations.Add(station);
            }

            Game.Console.Print("[San Andreas Patrol] Finished loading " + Stations.Count + " stations to memory!");
        }

        public static Station GetDefaultStation(string agency) {
            return Stations.FirstOrDefault(x => x.Agency == agency && x.Default) ?? Stations.FirstOrDefault(x => x.Agency == agency);
        }

        public static Station GetStationById(string id) {
            return Stations.Find(x => x.Id == id);
        }

        public static Station GetStationByName(string name) {
            return Stations.Find(x => x.Name == name);
        }

        public static List<string> GetStationNames() {
            List<string> names = new List<string>();

            foreach (Station station in Stations.Where(x => x.Disabled != true))
                names.Add(station.Name);

            return names;
        }

        public static List<string> GetStationNamesByAgency(string agency) {
            List<string> names = new List<string>();

            foreach (Station station in Stations.Where(x => x.Disabled != true && x.Agency == agency))
                names.Add(station.Name);

            return names;
        }
    }
}
