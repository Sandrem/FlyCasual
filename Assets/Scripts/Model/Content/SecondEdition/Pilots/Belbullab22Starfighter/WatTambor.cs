using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class WatTambor : Belbullab22Starfighter
    {
        public WatTambor()
        {
            PilotInfo = new PilotCardInfo(
                "Wat Tambor",
                3,
                44,
                true,
                abilityType: typeof(Abilities.SecondEdition.WatTamborAbility),
                pilotTitle: "Techno Union Foreman",
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5e/3d/5e3d8e36-3989-40f4-9908-6bd6583bb88a/swz29_wat-tambor.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WatTamborAbility : GenericAbility
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
