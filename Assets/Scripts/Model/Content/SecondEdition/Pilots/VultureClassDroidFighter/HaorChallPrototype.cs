using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class HaorChallPrototype : VultureClassDroidFighter
    {
        public HaorChallPrototype()
        {
            PilotInfo = new PilotCardInfo(
                "Haor Chall Prototype",
                1,
                23,
                abilityType: typeof(Abilities.SecondEdition.HaorChallPrototypeAbility),
                pilotTitle: "Xi Char Offering"
            );

            // TODO: Limited 2 

            ModelInfo.SkinName = "Gray";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/04/05/0405890a-0f0a-444e-b9eb-8d92dbdf3d63/swz29_hadr-chall.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HaorChallPrototypeAbility : GenericAbility
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
