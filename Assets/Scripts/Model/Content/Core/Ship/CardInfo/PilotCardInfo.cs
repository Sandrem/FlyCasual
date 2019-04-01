using System;
using System.Collections.Generic;
using Upgrade;
using static Ship.GenericShip;

namespace Ship
{
    public enum ForceAlignment
    {
        None,
        Light,
        Dark
    }

    public class PilotCardInfo
    {
        public string PilotName { get; private set; }
        public string PilotTitle { get; private set; }
        public int Initiative { get; private set; }

        public int Limited { get; private set; }
        public bool IsLimited { get { return Limited != 0; } }

        public Type AbilityType { get; private set; }

        public int Force { get; set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }

        public int Cost { get; private set; }

        public List<UpgradeType> ExtraUpgrades { get; private set; }
        public Faction Faction { get; private set; }
        public int SEImageNumber { get; private set; }

        public PilotCardInfo(string pilotName, int initiative, int cost, bool isLimited = false, int limited = 0, Type abilityType = null, string pilotTitle = "", int force = 0, int charges = 0, bool regensCharges = false, UpgradeType extraUpgradeIcon = UpgradeType.None, List<UpgradeType> extraUpgradeIcons = null, Faction factionOverride = Faction.None, int seImageNumber = 0)
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;

            if (isLimited)
            {
                Limited = 1;
            }
            else
            {
                Limited = limited;
            }

            AbilityType = abilityType;

            Force = force;
            Charges = charges;
            RegensCharges = regensCharges;

            Cost = cost;

            SEImageNumber = seImageNumber;

            ExtraUpgrades = new List<UpgradeType>();
            if (extraUpgradeIcon != UpgradeType.None) ExtraUpgrades.Add(extraUpgradeIcon);
            if (extraUpgradeIcons != null) ExtraUpgrades.AddRange(extraUpgradeIcons);
            if (factionOverride != Faction.None) Faction = factionOverride;
        }

        public delegate void EventHandlerForceAlignmentBool(ForceAlignment alignment, ref bool data);
        public event EventHandlerForceAlignmentBool OnForceAlignmentEquipCheck;

        public bool CanEquipForceAlignedCard(ForceAlignment alignment)
        {
            var result = false;

            switch (alignment)
            {
                case ForceAlignment.Light:
                    result = Faction == Faction.Republic ||
                             Faction == Faction.Rebel ||
                             Faction == Faction.Resistance;
                    break;
                case ForceAlignment.Dark:
                    result = Faction == Faction.Separatists ||
                             Faction == Faction.Imperial ||
                             Faction == Faction.FirstOrder ||
                             Faction == Faction.Scum;
                    break;
                default:
                    result =  true;
                    break;
            }

            OnForceAlignmentEquipCheck?.Invoke(alignment, ref result);

            return result;
        }
    }
}
