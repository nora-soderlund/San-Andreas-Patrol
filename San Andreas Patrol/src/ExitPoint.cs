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

namespace SanAndreasPatrol {
    public static class ExitPoint {
        public static void Exit(bool isTerminating) {
            PluginSettings.Save();

            CareerManager.Stop();
        }
    }
}
