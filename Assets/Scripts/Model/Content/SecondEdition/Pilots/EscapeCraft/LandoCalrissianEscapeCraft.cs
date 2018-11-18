using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class LandoCalrissianEscapeCraft : EscapeCraft
        {
            public LandoCalrissianEscapeCraft() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    4,
                    26,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 226;
            }
        }
    }
}