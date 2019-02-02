using Upgrade;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class TrajectorySimulator : GenericUpgrade
    {
        public TrajectorySimulator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Trajectory Simulator",
                UpgradeType.System,
                cost: 10,
                abilityType: typeof(Abilities.FirstEdition.TrajectorySimulatorAbility),
                seImageNumber: 26
            );
        }        
    }
}