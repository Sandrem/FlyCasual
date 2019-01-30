using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class PreciseHunter : VultureClassDroidFighter
    {
        public PreciseHunter()
        {
            PilotInfo = new PilotCardInfo(
                "Precise Hunter",
                3,
                27,
                abilityType: typeof(Abilities.SecondEdition.PreciseHunterAbility),
                pilotTitle: "Pinpoint Protocols"
            );

            // TODO: Limited 3 

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/37/0c/370c5cb2-0f0d-4d6f-9358-eb3cad9088dc/swz29_precise-hunter.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PreciseHunterAbility : GenericAbility
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
