using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class WedgeAntilles : T65XWing
        {
            public WedgeAntilles() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wedge Antilles",
                    6,
                    52,
                    limited: 1,
                    abilityText: "While you perform an attack, the defender rolls 1 fewer defense die.",
                    abilityType: typeof(Abilities.FirstEdition.WedgeAntillesAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 1;
            }
        }
    }
}
