using System;
using System.Collections.Generic;

namespace Ship
{
    public class PilotCardInfo
    {
        public string PilotName { get; private set; }
        public string PilotTitle { get; private set; }
        public int Initiative { get; set; }
        public int Limited { get; private set; }

        public string AbilityText { get; private set; }
        public Type AbilityType { get; private set; }

        public int Force { get; private set; }
        public int Energy { get; private set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }

        public int Cost { get; set; }

        public PilotCardInfo(string pilotName, int initiative, int cost, int limited = 0, string abilityText = "", Type abilityType = null, string pilotTitle = "", int force = 0, int energy = 0, int charges = 0, bool regensCharges = false)
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;
            Limited = limited;

            AbilityText = abilityText;
            AbilityType = abilityType;

            Force = force;
            Energy = energy;
            Charges = charges;
            RegensCharges = regensCharges;

            Cost = cost;
        }
    }
}
