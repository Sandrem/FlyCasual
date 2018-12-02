using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AttackShuttle
    {
        public class HeraSyndulla : AttackShuttle
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    7,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}