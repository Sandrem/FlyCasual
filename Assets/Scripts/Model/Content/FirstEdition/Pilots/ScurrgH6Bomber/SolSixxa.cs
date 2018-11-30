using Bombs;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ScurrgH6Bomber
    {
        public class SolSixxa : ScurrgH6Bomber
        {
            public SolSixxa() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sol Sixxa",
                    6,
                    28,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SolSixxaAbiliity),
                    extraUpgradeIcon: UpgradeType.Elite
                );
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
