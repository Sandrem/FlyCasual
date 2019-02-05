using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class MaceWindu : Delta7Aethersprite
    {
        public MaceWindu()
        {
            PilotInfo = new PilotCardInfo(
                "Mace Windu",
                5,
                58,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/89/28/8928ae70-8883-4c39-9b15-a4754c063b88/swz32_mace-windu.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MaceWinduAbility : GenericAbility
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
