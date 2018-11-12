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

        public int Cost { get; set; }

        public PilotCardInfo(string pilotName, int initiative, string abilityText, int cost, int limited = 0, Type abilityType = null, string pilotTitle = "")
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;
            Limited = limited;

            AbilityText = abilityText;
            AbilityType = abilityType;

            Cost = cost;
        }
    }
}
