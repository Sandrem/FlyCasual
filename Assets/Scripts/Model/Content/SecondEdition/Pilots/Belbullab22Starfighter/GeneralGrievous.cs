using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class GeneralGrievous : Belbullab22Starfighter
    {
        public GeneralGrievous()
        {
            PilotInfo = new PilotCardInfo(
                "General Grievous",
                4,
                46,
                true,
                abilityType: typeof(Abilities.SecondEdition.GeneralGrievousAbility),
                pilotTitle: "Ambitious Cyborg",
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e1/9e/e19e3aaa-4b9f-4a9e-bc8f-46812882ebc7/swz29_grievous.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GeneralGrievousAbility : GenericAbility
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
