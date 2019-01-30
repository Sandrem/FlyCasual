using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class DarthMaulPilot: SithInfiltrator
    {
        public DarthMaulPilot()
        {
            PilotInfo = new PilotCardInfo(
                "Darth Maul",
                5,
                70,
                true,
                abilityType: typeof(Abilities.SecondEdition.DarthMaulPilotAbility),
                pilotTitle: "Sith Assassin",
                force: 3,
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/35/d8/35d8295c-1018-4ed7-94a0-c0bff4e6fbbc/swz30_darth-maul.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DarthMaulPilotAbility : GenericAbility
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
