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

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Career;

using SanAndreasPatrol.Radio.Menu;
using SanAndreasPatrol.Radio.Tasks;

namespace SanAndreasPatrol.Radio {
    class RadioManager {
        public static IRadioTask Task;

        public static void StartTask(IRadioTask task) {
            StopTask();

            Task = task;
        }

        public static void StopTask() {
            if (Task == null)
                return;

            Task = null;
        }

        public static List<RadioMessage> Messages = new List<RadioMessage>();

        public static void Fiber() {
            XDocument xDocument = XDocument.Load("plugins/San Andreas Patrol/data/radios.xml");

            XElement xRadios = xDocument.Element("Radios");

            foreach(XElement xRadio in xRadios.Elements("Radio")) {
                RadioMessage radioMessage = new RadioMessage() {
                    Id = xRadio.Attribute("id").Value
                };

                foreach(XElement xAgency in xRadio.Elements("Agency")) {
                    RadioMessageAgency radioMessageAgency = new RadioMessageAgency() {
                        Agency = AgencyManager.GetAgencyById(xAgency.Attribute("id").Value),
                        Default = (xAgency.Attribute("default") != null),

                        Message = xAgency.Value
                    };

                    radioMessage.Agencies.Add(radioMessageAgency);
                }

                Messages.Add(radioMessage);
            }

            RadioMenu.Fiber();
        }

        public static uint Dispatch(string id, Dictionary<string, string> replacements) {
            RadioMessage radioMessage = Messages.Find(x => x.Id == id);

            RadioMessageAgency radioMessageAgency = radioMessage.Agencies.Find(x => x.Agency == CareerManager.Career.Agency);

            if (radioMessageAgency == null)
                radioMessageAgency = radioMessage.Agencies.FirstOrDefault();

            string result = radioMessageAgency.Message;

            foreach(KeyValuePair<string, string> replacement in replacements)
                result = result.Replace("${" + replacement.Key + "}", replacement.Value);

            return Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "Control Operator", "Communications Division", result);
        }

        public static void Request(string id, Dictionary<string, string> replacements, Action action) {
            RadioMessage radioMessage = Messages.Find(x => x.Id == id);

            RadioMessageAgency radioMessageAgency = radioMessage.Agencies.Find(x => x.Agency == CareerManager.Career.Agency);

            if (radioMessageAgency == null)
                radioMessageAgency = radioMessage.Agencies.FirstOrDefault();

            string result = radioMessageAgency.Message;

            foreach (KeyValuePair<string, string> replacement in replacements)
                result = result.Replace("${" + replacement.Key + "}", replacement.Value);

            RadioMenu.Replies.Add(result, action);

            RadioMenu.Update();
        }

        public static void ClearRequests() {
            RadioMenu.Replies.Clear();

            RadioMenu.Update();
        }
    }
}
