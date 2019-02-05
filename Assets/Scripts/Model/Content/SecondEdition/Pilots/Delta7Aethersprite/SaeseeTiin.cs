using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class SaeseeTiin : Delta7Aethersprite
    {
        public SaeseeTiin()
        {
            PilotInfo = new PilotCardInfo(
                "Saesee Tiin",
                4,
                58,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.SaeseeTiinAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ModelInfo.SkinName = "Saesee Tiin";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/77/73/777350cb-614b-48fd-ad8d-d9c867053c6b/swz32_saesee-tiin.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SaeseeTiinAbility : GenericAbility
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
