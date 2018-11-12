using System.Collections;
using System.Collections.Generic;
using Abilities.FirstEdition;

namespace Ship
{
    namespace SecondEdition.XWing
    {
        public class EdrioTwoTubes : XWing
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