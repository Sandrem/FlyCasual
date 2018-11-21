using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wullffwarro",
                    4,
                    56,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WullffwarroAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 31;
            }
        }
    }
}
