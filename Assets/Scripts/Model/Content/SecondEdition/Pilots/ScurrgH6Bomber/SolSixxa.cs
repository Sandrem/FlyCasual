using BoardTools;
using Bombs;
using Movement;
using System.Collections.Generic;
using System.Linq;
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
                    46,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SolSixxaAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
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
        protected override void SolSixxaTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            base.SolSixxaTemplate(availableTemplates, upgrade);

            List<ManeuverTemplate> newTemplates = new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1, isBombTemplate: true),
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