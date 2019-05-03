using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class OddBall : BTLBYWing
        {
            public OddBall() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Odd Ball\"",
                    5,
                    35,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityText: "After you fully execute a red maneuver or perform a red action, if there is an enemy ship in your bullseye arc, you may acquire a lock on that ship.",
                    abilityType: typeof(Abilities.SecondEdition.OddBallAbility)
                );
            }
        }
    }
}
