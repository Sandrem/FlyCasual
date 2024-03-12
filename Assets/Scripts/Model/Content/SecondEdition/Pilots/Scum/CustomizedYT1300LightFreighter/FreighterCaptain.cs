using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class FreighterCaptain : CustomizedYT1300LightFreighter
        {
            public FreighterCaptain() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Freighter Captain",
                    "",
                    Faction.Scum,
                    1,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    },
                    seImageNumber: 225
                );
            }
        }
    }
}
