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
                    35,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Astromech }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6c/73/6c73ec27-bf29-4ace-86e9-4d03cdafa884/swz48_pilot-shadow-vet.png";
            }
        }
    }
}
