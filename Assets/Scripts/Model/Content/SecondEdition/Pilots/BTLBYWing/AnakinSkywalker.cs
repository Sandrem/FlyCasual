using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class AnakinSkywalker : BTLBYWing
        {
            public AnakinSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Anakin Skywalker",
                    6,
                    40,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Force,
                    force: 3,
                    abilityText: "After you fully execute a maneuver, if there is an enemy ship in your standard front arc at range 0-1 or in your bullseye arc, you may spend 1 force to remove 1 stress token.",
                    abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility)
                );
            }
        }
    }
}
