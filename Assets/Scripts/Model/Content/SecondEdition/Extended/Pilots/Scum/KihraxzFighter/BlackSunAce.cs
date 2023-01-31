using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class BlackSunAce : KihraxzFighter
        {
            public BlackSunAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Sun Ace",
                    "",
                    Faction.Scum,
                    3,
                    4,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent
                    },
                    seImageNumber: 195,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Black Sun (White)";
            }
        }
    }
}
