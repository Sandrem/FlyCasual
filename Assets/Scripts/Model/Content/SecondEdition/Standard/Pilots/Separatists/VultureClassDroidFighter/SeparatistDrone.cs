using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class SeparatistDrone : VultureClassDroidFighter
    {
        public SeparatistDrone()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Separatist Drone",
                "",
                Faction.Separatists,
                3,
                2,
                3,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/89/cb/89cb527c-4578-410c-9e5b-4ac78815a679/swz31_separatist-drone.png";
        }
    }
}