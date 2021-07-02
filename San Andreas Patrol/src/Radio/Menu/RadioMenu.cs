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

namespace SanAndreasPatrol.Radio.Menu {
    class RadioMenu {
        public static UIMenu Menu;

        public static Dictionary<string, Action> Replies = new Dictionary<string, Action>();

        public static void Fiber() {
            Menu = new UIMenu("Communications", "Communications Division");

            EntryPoint.MenuPool.Add(Menu);

            KeyboardData.Register(Keys.N, OnKeyDown);
        }

        public static void Update() {
            Menu.Clear();

            foreach(KeyValuePair<string, Action> reply in Replies) {
                UIMenuItem replyItem = new UIMenuItem(reply.Key);

                replyItem.Activated += (e, s) => {
                    Menu.Visible = false;

                    Replies.Clear();

                    Update();

                    reply.Value();
                };

                Menu.AddItem(replyItem);
            }

            if (Replies.Count != 0)
                Menu.AddItem(new UIMenuItem(" ") { Enabled = false });

            UIMenuItem backup = new UIMenuItem("Backup Options");
            Menu.AddItem(backup);
        }

        public static void OnKeyDown() {
            if (CareerManager.Career == null)
                return;

            if (UIMenu.IsAnyMenuVisible || TabView.IsAnyPauseMenuVisible)
                return;

            Menu.Visible = !Menu.Visible;
        }
    }
}
