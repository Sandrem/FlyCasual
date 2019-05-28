using Upgrade;
using Ship;
using Arcs;
using BoardTools;
using System.Collections.Generic;
using Movement;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SkilledBombardier : GenericUpgrade
    {
        public SkilledBombardier() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Skilled Bombardier",
                UpgradeType.Gunner,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.SkilledBombardierAbility),
                seImageNumber: 50
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SkilledBombardierAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesModifications += SkilledBombardierTemplate;
            HostShip.OnGetAvailableBombLaunchTemplatesModifications += SkilledBombardierTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesModifications -= SkilledBombardierTemplate;
            HostShip.OnGetAvailableBombLaunchTemplatesModifications -= SkilledBombardierTemplate;
        }

        protected virtual void SkilledBombardierTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> AddedTemplates = new List<ManeuverTemplate>();

            foreach (ManeuverTemplate availableTemplate in availableTemplates)
            {
                ManeuverSpeed reducedSpeed = availableTemplate.Speed - 1;
                ManeuverTemplate reducedSpeedTemplate = new ManeuverTemplate(availableTemplate.Bearing, availableTemplate.Direction, reducedSpeed, isBombTemplate: true);
                if (reducedSpeedTemplate.ValidTemplate()
                    && !availableTemplates.Any(t => t.Name == reducedSpeedTemplate.Name)
                    && !AddedTemplates.Any(t => t.Name == reducedSpeedTemplate.Name)
                    )
                {
                    AddedTemplates.Add(reducedSpeedTemplate);
                }

                ManeuverSpeed increasedSpeed = availableTemplate.Speed + 1;
                ManeuverTemplate increasedSpeedTemplate = new ManeuverTemplate(availableTemplate.Bearing, availableTemplate.Direction, increasedSpeed, isBombTemplate: true);
                if (increasedSpeedTemplate.ValidTemplate()
                    && !availableTemplates.Any(t => t.Name == increasedSpeedTemplate.Name)
                    && !AddedTemplates.Any(t => t.Name == increasedSpeedTemplate.Name)
                    )
                {
                    AddedTemplates.Add(increasedSpeedTemplate);
                }
            }

            availableTemplates.AddRange(AddedTemplates);
        }
    }
}