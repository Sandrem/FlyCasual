using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class SeparatistBomber : HyenaClassDroidBomber
    {
        public SeparatistBomber()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Separatist Bomber",
                "",
                Faction.Separatists,
                3,
                3,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5d/88/5d88fdec-55d6-4ea4-9fa1-af79d0ca5fcd/swz41_separatist-bomber.png";
        }
    }
}