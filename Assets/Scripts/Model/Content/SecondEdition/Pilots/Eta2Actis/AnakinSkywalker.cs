using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class AnakinSkywalker : Eta2Actis
    {
        public AnakinSkywalker()
        {
            PilotInfo = new PilotCardInfo(
                "Anakin Skywalker",
                6,
                62,
                true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerActisAbility),
                extraUpgradeIcon: UpgradeType.ForcePower
            );

            ModelInfo.SkinName = "Yellow";

            PilotNameCanonical = "anakinskywalker-eta2actis";

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a0/Anakin_eta-2.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AnakinSkywalkerActisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
