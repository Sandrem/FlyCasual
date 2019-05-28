using BoardTools;
using Bombs;
using Movement;
using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    extraUpgradeIcon: UpgradeType.Talent
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

        protected virtual void SolSixxaTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> newTemplates = new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed1, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed1, isBombTemplate: true),
            };

            foreach (ManeuverTemplate newTemplate in newTemplates)
            {
                if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
                {
                    availableTemplates.Add(newTemplate);
                }
            }

        }
    }
}
