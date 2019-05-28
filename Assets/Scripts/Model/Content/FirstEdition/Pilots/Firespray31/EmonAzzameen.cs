using BoardTools;
using Bombs;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class EmonAzzameen : Firespray31
        {
            public EmonAzzameen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Emon Azzameen",
                    6,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.EmonAzzameenAbility),
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum
                );

                ModelInfo.SkinName = "Emon Azzameen";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class EmonAzzameenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates += AddEmonAzzameenTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplates -= AddEmonAzzameenTemplates;
        }

        private void AddEmonAzzameenTemplates(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> newTemplates = new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed3, isBombTemplate: true),
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