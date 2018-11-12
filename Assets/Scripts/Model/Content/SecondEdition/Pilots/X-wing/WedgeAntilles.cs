using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.XWing
    {
        public class WedgeAntilles : XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    6,
                    "While you perform an attack, the defender rolls 1 fewer defense die.",
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.WedgeAntillesAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 1;
            }
        }
    }
}
