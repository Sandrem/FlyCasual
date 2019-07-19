using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;

namespace Remote
{
    public class RemoteState
    {
        public int Initiative { get; private set; }
        public int Agility { get; private set; }
        public int HullMax { get; private set; }
        public int HullCurrent { get; private set; }
    }
}
