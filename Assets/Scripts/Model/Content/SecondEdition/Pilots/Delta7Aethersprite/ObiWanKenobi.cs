using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class ObiWanKenobi : Delta7Aethersprite
    {
        public ObiWanKenobi()
        {
            PilotInfo = new PilotCardInfo(
                "Obi-Wan Kenobi",
                5,
                65,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.ObiWanKenobiAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_Obi-wan-Kenobi_pl.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ObiWanKenobiAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            // TODO
        }

        public override void DeactivateAbility()
        {
            // TODO
        }
    }
}
