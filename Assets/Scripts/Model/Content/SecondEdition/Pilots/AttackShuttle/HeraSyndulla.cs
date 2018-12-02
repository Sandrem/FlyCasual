using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class HeraSyndulla : AttackShuttle
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    39,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    force: 1,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 34
                );
            }
        }
    }
}