using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class LoyalistVolunteer : NimbusClassVWing
        {
            public LoyalistVolunteer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Loyalist Volunteer",
                    "",
                    Faction.Republic,
                    2,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a1/27/a127938a-aecc-48eb-ba09-622781e84084/swz80_ship_volunteer.png";
            }
        }
    }
}