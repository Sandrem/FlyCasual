using Upgrade;
using Abilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UpgradesList
{
    public class TrajectorySimulator : GenericUpgrade
    {
        public TrajectorySimulator() : base()
        {
            Type = UpgradeType.System;
            Name = "Trajectory Simulator";
            Cost = 1;

            UpgradeAbilities.Add (new TrajectorySimulatorAbility());
        }
    }
}

namespace Abilities
{
    public class TrajectorySimulatorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanLaunchBombs = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanLaunchBombs = false;
        }
    }
}
