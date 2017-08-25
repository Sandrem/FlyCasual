using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipPrintedInfo
    {
        public string ImageUrl { get; protected set; }
        public string ManeuversImageUrl { get; protected set; }

        public string ShipType { get; protected set; }
        public Faction Faction { get; protected set; }

        public string PilotName { get; protected set; }
        public bool IsUnique { get; protected set; }

        public int PilotSkill { get; protected set; }

        public int Firepower { get; protected set; }
        public int Agility { get; protected set; }
        public int MaxHull { get; protected set; }
        public int MaxShields { get; protected set; }

        // ActionsBar

        // UpgradesBar

        // Maneuvers

        // BaseSize

        // ArctType

        public int Cost { get; protected set; }

    }
}
