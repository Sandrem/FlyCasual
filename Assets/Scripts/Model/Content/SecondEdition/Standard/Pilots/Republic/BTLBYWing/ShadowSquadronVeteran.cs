using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Shadow Squadron Veteran",
                    "",
                    Faction.Republic,
                    3,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.YWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6c/73/6c73ec27-bf29-4ace-86e9-4d03cdafa884/swz48_pilot-shadow-vet.png";
            }
        }
    }
}
