using System;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol {
    class KeyboardData {
        public static Dictionary<Keys, List<Action>> Keys = new Dictionary<Keys, List<Action>>();

        public static List<Keys> KeysDown = new List<Keys>();

        public static void Fiber() {
            while(true) {
                GameFiber.Yield();

                foreach(KeyValuePair<Keys, List<Action>> pair in Keys) {
                    if(KeysDown.Contains(pair.Key)) {
                        if(!Game.IsKeyDownRightNow(pair.Key))
                            KeysDown.Remove(pair.Key);

                        continue;
                    }

                    KeysDown.Add(pair.Key);

                    if (Game.IsKeyDownRightNow(pair.Key)) {
                        foreach(Delegate method in pair.Value) {
                            method.DynamicInvoke();
                        }
                    }
                }
            }
        }

        public static void Register(Keys key, Action method) {
            if (!Keys.ContainsKey(key))
                Keys.Add(key, new List<Action>());

            Keys[key].Add(method);
        }
    }
}
