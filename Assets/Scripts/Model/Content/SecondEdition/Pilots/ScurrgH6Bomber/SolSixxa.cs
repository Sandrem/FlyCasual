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
                    abilityType: typeof(Abilities.SecondEdition.SolSixxaAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 205;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class SolSixxaAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += SolSixxaTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= SolSixxaTemplate;
        }

        protected virtual void SolSixxaTemplate(List<BombDropTemplates> availableTemplates)
        {
            if (!availableTemplates.Contains(BombDropTemplates.Turn_1_Left)) availableTemplates.Add(BombDropTemplates.Turn_1_Left);
            if (!availableTemplates.Contains(BombDropTemplates.Turn_1_Right)) availableTemplates.Add(BombDropTemplates.Turn_1_Right);
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