using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class EzraBridger : SheathipedeClassShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    3,
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Force);

                SEImageNumber = 39;
            }
        }
    }
}
