using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipStateInfo
    {
        public int Initiative { get; private set; }

        public int Firepower { get; set; }

        private int agility;
        public int Agility
        {
            get
            {
                int result = agility;
                result = Mathf.Max(result, 0);
                return result;
            }

            set
            {
                agility = value;
            }
        }

        public int HullMax { get; private set; }
        public int HullCurrent { get; private set; }
        public int ShieldsMax { get; private set; }
        public int ShieldsCurrent { get; private set; }
    }
}
