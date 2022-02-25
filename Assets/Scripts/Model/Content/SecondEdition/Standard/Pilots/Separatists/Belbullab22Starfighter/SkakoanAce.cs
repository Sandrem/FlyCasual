using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class SkakoanAce : Belbullab22Starfighter
    {
        public SkakoanAce()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Skakoan Ace",
                "",
                Faction.Separatists,
                3,
                4,
                4,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification
                }
            );

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/ceb7a3bc406ff17be5dee5de62b39195.png";
        }
    }
}