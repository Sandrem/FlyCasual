using BoardTools;
using Movement;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.QuadrijetTransferSpacetug
    {
        public class ConstableZuvio : QuadrijetTransferSpacetug
        {
            public ConstableZuvio() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Constable Zuvio",
                    4,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ConstableZuvioAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 161
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ConstableZuvioAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombLaunchTemplates += ConstableZuvioTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombLaunchTemplates -= ConstableZuvioTemplate;
        }

        protected virtual void ConstableZuvioTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            ManeuverTemplate newTemplate = new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1);

            if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
            {
                availableTemplates.Add(newTemplate);
            }
        }
    }
}
