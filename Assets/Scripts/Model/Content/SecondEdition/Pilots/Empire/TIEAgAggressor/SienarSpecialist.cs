using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class SienarSpecialist : TIEAgAggressor
        {
            public SienarSpecialist() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sienar Specialist",
                    "",
                    Faction.Imperial,
                    2,
                    4,
                    8,
                    seImageNumber: 130,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Turret,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
