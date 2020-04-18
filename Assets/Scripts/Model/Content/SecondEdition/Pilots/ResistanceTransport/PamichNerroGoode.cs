using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.SecondEdition;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransport
{
    public class PammichNerroGoode : ResistanceTransport
    {
        public PammichNerroGoode()
        {
            PilotInfo = new PilotCardInfo(
                "Pammich Nerro Goode",
                3,
                36,
                isLimited: true,
                abilityType: typeof(PammichNerroGoodeAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/8b/2d/8b2da8f2-041a-43e0-a48d-84fc958eb05e/swz45_pammich-goode.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PammichNerroGoodeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed += CheckRedManeuversWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= CheckRedManeuversWhileStressed;
        }

        private void CheckRedManeuversWhileStressed(ref bool isAllowed)
        {
            if (HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) <= 2)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Red maneuver is allowed");
                isAllowed = true;
            }
        }
    }
}