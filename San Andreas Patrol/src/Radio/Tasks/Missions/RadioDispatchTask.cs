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

using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

using SanAndreasPatrol.Agencies;
using SanAndreasPatrol.Career;
using SanAndreasPatrol.Extras;
using SanAndreasPatrol.Radio;

namespace SanAndreasPatrol.Radio.Tasks.Missions {
    class RadioDispatchTask : IRadioTask {
        public RadioDispatchTask(string mission, bool code3, bool code2, Dictionary<string, string> parameters) {
            RadioManager.Dispatch(mission + "_DISPATCH", parameters);

            if (code3) {
                RadioManager.Request("DISPATCH_CODE_3", parameters, () => {
                    RadioManager.Dispatch("DISPATCH_CODE_3", parameters);

                    Game.DisplaySubtitle("Respond to the ~r~area~w~ with lights and sirens.");
                });
            }

            if (code2) {
                RadioManager.Request("DISPATCH_CODE_2", parameters, () => {
                    RadioManager.Dispatch("DISPATCH_CODE_2", parameters);

                    Game.DisplaySubtitle("Respond to the ~r~area~w~ without lights and sirens.~n~Upgrade to lights and sirens in the radio menu.");

                    RadioManager.Request("DISPATCH_CODE_3_UPGRADE", parameters, () => {
                        RadioManager.Dispatch("DISPATCH_CODE_3_UPGRADE", parameters);

                        Game.DisplaySubtitle("Respond to the ~r~area~w~ with lights and sirens.");
                    });
                });
            }
        }
    }
}
