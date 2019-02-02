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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.EmonAzzameenAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 150
                );
            }
        }
    }
}