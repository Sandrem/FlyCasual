using BoardTools;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Content;

namespace UpgradesList.SecondEdition
{
    public class TrajectorySimulator : GenericUpgrade
    {
        public TrajectorySimulator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trajectory Simulator",
                UpgradeType.Sensor,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.TrajectorySimulatorAbility),
                seImageNumber: 26,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TrajectorySimulatorAbility : Abilities.FirstEdition.TrajectorySimulatorAbility
    {
        protected override void TrajectorySimulatorTemplate(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            if (Phases.CurrentPhase.GetType() != typeof(MainPhases.SystemsPhase)) return;

            if (upgrade.UpgradeInfo.SubType != UpgradeSubType.Bomb) return;

            ManeuverTemplate newTemplate = new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed5);

            if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
            {
                availableTemplates.Add(newTemplate);
            }
        }
    }
}