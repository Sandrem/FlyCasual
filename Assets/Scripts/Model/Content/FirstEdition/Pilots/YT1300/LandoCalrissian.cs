using ActionsList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class LandoCalrissian : YT1300
        {
            public LandoCalrissian() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lando Calrissian",
                    7,
                    44,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.LandoCalrissianRebelPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Missile }
                );

                ShipInfo.ArcInfo.Arcs.ForEach(a => a.Firepower = 3);
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;
            }
        }
    }
}

