using Upgrade;
using System.Collections.Generic;
using System.Linq;
using Ship;
using ActionsList;
using Obstacles;
using BoardTools;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class DebrisGambit : GenericUpgrade
    {
        public DebrisGambit() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Debris Gambit",
                UpgradeType.Talent,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.DebrisGambit),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium),
                addAction: new ActionInfo(typeof(EvadeAction), ActionColor.Red),
                seImageNumber: 3
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DebrisGambit : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckActionComplexity += CheckDecreaseComplexity;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckActionComplexity -= CheckDecreaseComplexity;
        }

        private void CheckDecreaseComplexity(GenericAction action, ref ActionColor color)
        {
            if (action is EvadeAction && color == ActionColor.Red)
            {
                if (IsNearObstacle()) color = ActionColor.White;
            }
        }

        private bool IsNearObstacle()
        {
            if (HostShip.IsLandedOnObstacle)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": The Evade action is treated as a white action");
                return true;
            }

            foreach (GenericObstacle obstacle in ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance shipObstacleDist = new ShipObstacleDistance(HostShip, obstacle);
                if (shipObstacleDist.Range < 2)
                {
                    Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": The Evade action is treated as a white action");
                    return true;
                }
            }

            return false;
        }
    }
}