using System.Collections;
using System.Collections.Generic;
using Abilities.FirstEdition;
using BoardTools;
using Arcs;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class GideonHask : TIELnFighter
        {
            public GideonHask() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gideon Hask",
                    4,
                    30,
                    limited: 1,
                    abilityType: typeof(ScourgeAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 84;
            }
        }
    }
}

