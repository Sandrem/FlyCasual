using Bombs;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScurrgH6Bomber
    {
        public class SolSixxa : ScurrgH6Bomber
        {
            public SolSixxa() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sol Sixxa",
                    3,
                    49,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.SolSixxaAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 205
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SolSixxaAbility : Abilities.FirstEdition.SolSixxaAbiliity
    {
        protected override void SolSixxaTemplate(List<BombDropTemplates> availableTemplates)
        {
            base.SolSixxaTemplate(availableTemplates);

            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Left)) availableTemplates.Add(BombDropTemplates.Bank_1_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Bank_1_Right)) availableTemplates.Add(BombDropTemplates.Bank_1_Right);
        }
    }
}