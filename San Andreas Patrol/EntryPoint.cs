using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

[assembly: Rage.Attributes.Plugin("San Andreas Patrol", Description = "An immersive and realistic law enforcement roleplay plugin.", Author = "Chloe Ohlsson")]

namespace SanAndreasPatrol {
    public class Character {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Agency { get; set; }
    }


    public static class EntryPoint {
        private static void Main() {
            foreach(var instance in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(a => a.GetConstructor(Type.EmptyTypes) != null).Select(Activator.CreateInstance).OfType<IEntryPoint>()) {
                GameFiber.StartNew(instance.OnFiberStart);
            }

            while(true) {
                GameFiber.Yield();
            }
        }
    }
}
