using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class CaptainSear : Belbullab22Starfighter
    {
        public CaptainSear()
        {
            PilotInfo = new PilotCardInfo(
                "Captain Sear",
                2,
                42,
                true,
                abilityType: typeof(Abilities.SecondEdition.CaptainSearAbility),
                pilotTitle: "Kage Infiltrator"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/76/ba/76baabac-2258-4d60-9cf9-d7b0cdf0faeb/swz29_captain-sear.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainSearAbility : GenericAbility
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
