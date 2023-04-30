using Upgrade;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class BravoFlightOfficer : NabooRoyalN1Starfighter
        {
            public BravoFlightOfficer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bravo Flight Officer",
                    "",
                    Faction.Republic,
                    2,
                    4,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo
                    }
                );
            }
        }
    }
}
