using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Ezra Bridger",
                    "Spectre-6",
                    Faction.Rebel,
                    3,
                    4,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower
                    },
                    tags: new List<Tags>
                    {
                        Tags.Spectre,
                        Tags.LightSide
                    },
                    seImageNumber: 39
                );

                PilotNameCanonical = "ezrabridger-sheathipedeclassshuttle";
            }
        }
    }
}
