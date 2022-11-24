using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class LogisticsDivisionPilot : ResistanceTransport
    {
        public LogisticsDivisionPilot()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Logistics Division Pilot",
                "",
                Faction.Resistance,
                1,
                4,
                6,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Cannon,
                    UpgradeType.Crew,
                    UpgradeType.Astromech,
                    UpgradeType.Astromech
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/95/69/95692636-a7ef-499c-afb7-5891b998f696/swz45_logistics-pilot.png";
        }
    }
}