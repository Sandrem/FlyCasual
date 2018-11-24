using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class FennRau : SheathipedeClassShuttle
        {
            public FennRau() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fenn Rau",
                    6,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.FennRauRebelAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 38
                );
            }
        }
    }
}
