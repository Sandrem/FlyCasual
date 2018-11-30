using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class Kavil : BTLA4YWing
        {
            public Kavil() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kavil",
                    5,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KavilAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 165
                );
            }
        }
    }
}