using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

using Rage;
using Rage.Native;
using Rage.Attributes;

using SanAndreasPatrol.Career;

namespace SanAndreasPatrol {
    class PluginSettings {
        public static string PreviousCareer = null;

        public static bool StartOnLoad = false;

        public static bool ProgressiveWorldTime = true;

        public static void Fiber() {
            if (!File.Exists("plugins/San Andreas Patrol/data/settings.xml"))
                Save();

            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/data/settings.xml");

            IEnumerable<XElement> xSettings = xDocument.Element("Settings").Elements("Setting");

            PreviousCareer = GetSetting(xSettings, "previousCareer");
            StartOnLoad = bool.Parse(GetSetting(xSettings, "startOnLoad"));
            ProgressiveWorldTime = bool.Parse(GetSetting(xSettings, "progressiveWorldTime"));
        }

        public static string GetSetting(IEnumerable<XElement> xElements, string id) {
            if (xElements.Count(x => x.Attribute("id").Value == id) == 0)
                return null;

            return xElements.Where(x => x.Attribute("id").Value == id).FirstOrDefault().Value;
        }

        public static void Save() {
            XDocument xDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),

                new XElement("Settings",
                    new XElement("Setting", new XAttribute("id", "previousCareer"), CareerManager.Career?.Id),
                    new XElement("Setting", new XAttribute("id", "startOnLoad"), StartOnLoad),
                    new XElement("Setting", new XAttribute("id", "progressiveWorldTime"), ProgressiveWorldTime)
                )
            );

            xDocument.Save("plugins/San Andreas Patrol/data/settings.xml");
        }
    }
}
