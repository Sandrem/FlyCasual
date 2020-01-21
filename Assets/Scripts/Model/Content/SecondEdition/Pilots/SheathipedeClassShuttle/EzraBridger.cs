using System.Collections;
using System.Collections.Generic;
using Upgrade;

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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1,
                    extraUpgradeIcon: UpgradeType.ForcePower,
                    seImageNumber: 39
                );

                PilotNameCanonical = "ezrabridger-sheathipedeclassshuttle";
            }
        }
    }
}
