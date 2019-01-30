using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class PloKoon : Delta7Aethersprite
    {
        public PloKoon()
        {
            PilotInfo = new PilotCardInfo(
                "Plo Koon",
                5,
                63,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://files.rebel.pl/images/wydawnictwo/zapowiedzi/Star_Wars/SWZ32_plo-koon_pl.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PloKoonAbility : GenericAbility
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
