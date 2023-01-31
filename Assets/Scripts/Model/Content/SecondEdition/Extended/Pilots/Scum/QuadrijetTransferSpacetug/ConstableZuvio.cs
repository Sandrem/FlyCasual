using BoardTools;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Constable Zuvio",
                    "Missing Sheriff of Niima Outpost",
                    Faction.Scum,
                    4,
                    4,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ConstableZuvioAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    },
                    seImageNumber: 161,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
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
