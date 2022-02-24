using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class AmaxineWarrior : BTLA4YWing
        {
            public AmaxineWarrior() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Amaxine Warrior",
                    "",
                    Faction.Scum,
                    3,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Astromech,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/52/7d/527dd63a-1c64-4d78-bac3-b7b999accaf9/swz85_pilot_amaxinewarrior.png";
            }
        }
    }
}
