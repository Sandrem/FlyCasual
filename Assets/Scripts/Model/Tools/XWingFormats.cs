using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upgrade;

namespace Content
{
    public static class XWingFormats
    {
        public static bool IsShipLegalForFormat(GenericShip ship)
        {
            if (Options.Format == "Standard") return (ship.ShipInfo as ShipCardInfo25).LegalityInfo.Contains(Legality.StandardLegal);
            else if (Options.Format == "Extended") return (ship.ShipInfo as ShipCardInfo25).LegalityInfo.Contains(Legality.ExtendedLegal);
            else return false;
        }

        public static bool IsLegalForFormat(GenericShip ship)
        {
            if (Options.Format == "Standard") return (ship.PilotInfo as PilotCardInfo25).LegalityInfo.Contains(Legality.StandardLegal);
            else if (Options.Format == "Extended") return (ship.PilotInfo as PilotCardInfo25).LegalityInfo.Contains(Legality.ExtendedLegal);
            else return false;
        }

        public static bool IsLegalForFormat(GenericUpgrade upgrade)
        {
            if (Options.Format == "Standard") return upgrade.UpgradeInfo.LegalityInfo.Contains(Legality.StandardLegal);
            else if (Options.Format == "Extended") return upgrade.UpgradeInfo.LegalityInfo.Contains(Legality.ExtendedLegal);
            else return false;
        }

        public static bool IsBanned(GenericShip ship)
        {
            if (Options.Format == "Standard") return (ship.PilotInfo as PilotCardInfo25).LegalityInfo.Contains(Legality.StandardBanned);
            else if (Options.Format == "Extended") return (ship.PilotInfo as PilotCardInfo25).LegalityInfo.Contains(Legality.ExtendedBanned);
            else return false;
        }

        public static bool IsBanned(GenericUpgrade upgrade)
        {
            if (Options.Format == "Standard") return upgrade.UpgradeInfo.LegalityInfo.Contains(Legality.StandardBanned);
            else if (Options.Format == "Extended") return upgrade.UpgradeInfo.LegalityInfo.Contains(Legality.ExtendedBanned);
            else return false;
        }
    }
}