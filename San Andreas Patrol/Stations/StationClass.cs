﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

using Rage;
using Rage.Attributes;

namespace SanAndreasPatrol.Stations {
    class Station {
        public string Id;
        public string Agency;

        public bool Default;
        public bool Disabled;

        public string Name;
        public string Description;
        public string Type;

        public Vector3 CameraPosition;
        public Rotator CameraRotation;

        public Dictionary<string, string> Images = new Dictionary<string, string>();
    }
}
