using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class BlackSunAssassin : StarViperClassAttackPlatform
        {
            public BlackSunAssassin() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Sun Assassin",
                    "",
                    Faction.Scum,
                    3,
                    5,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech
                    },
                    seImageNumber: 181,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Black Sun Assassin";
            }
        }
    }
}