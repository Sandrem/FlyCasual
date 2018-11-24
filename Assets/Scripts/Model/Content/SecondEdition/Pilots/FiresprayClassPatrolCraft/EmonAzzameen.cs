using Bombs;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class EmonAzzameen : FiresprayClassPatrolCraft
        {
            public EmonAzzameen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Emon Azzameen",
                    4,
                    76,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.EmonAzzameenAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 150
                );
            }
        }
    }
}