using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class ShadowSquadronVeteran : BTLBYWing
        {
            public ShadowSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Shadow Squadron Veteran",
                    3,
                    33,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b6/ca/b6cad4cc-120c-4637-8d49-3c9dba68f6ed/swz_w5_pilot-shadow-sqd-vet.png";
            }
        }
    }
}
