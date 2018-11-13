using System.Collections;
using System.Collections.Generic;
using Abilities.FirstEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class EdrioTwoTubes : T65XWing
        {
            public EdrioTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Edrio Two Tubes",
                    2,
                    45,
                    limited: 1,
                    abilityType: typeof(EdrioTwoTubesAbility)
                );

                ModelInfo.SkinName = "Partisan";

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Illicit);

                SEImageNumber = 9;
            }
        }
    }
}