using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class DisT81 : DroidTriFighter
    {
        public DisT81()
        {
            PilotInfo = new PilotCardInfo(
                "DIS-T81",
                3,
                36,
                true,
                abilityType: typeof(Abilities.SecondEdition.DisT81Ability)
            );

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c2/DIS-T81.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DisT81Ability : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
