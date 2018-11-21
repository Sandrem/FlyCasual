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
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KavilAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 165;
            }
        }
    }
}