using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Ship;
using ActionsList;
using Obstacles;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class DebrisGambit : GenericUpgrade
    {
        public DebrisGambit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Debris Gambit",
                UpgradeType.Elite,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.DebrisGambit),
                seImageNumber: 3
            );
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.BaseSize == BaseSize.Medium || ship.ShipInfo.BaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DebrisGambit : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddDebrisGambitAction;
            HostShip.OnCheckActionComplexity += CheckDecreaseComplexity;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddDebrisGambitAction;
            HostShip.OnCheckActionComplexity -= CheckDecreaseComplexity;
        }

        private void AddDebrisGambitAction(GenericShip host)
        {
            host.AddAvailableAction(new EvadeAction() { IsRed = true });
        }

        private void CheckDecreaseComplexity(ref GenericAction action)
        {
            if (action is EvadeAction && action.IsRed)
            {
                if (IsNearObstacle()) action.IsRed = false;
            }
        }

        private bool IsNearObstacle()
        {
            if (HostShip.IsLandedOnObstacle)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Action is treated as white");
                return true;
            }

            foreach (GenericObstacle obstacle in ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance shipObstacleDist = new ShipObstacleDistance(HostShip, obstacle);
                if (shipObstacleDist.Range < 2)
                {
                    Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Action is treated as white");
                    return true;
                }
            }

            return false;
        }
    }
}