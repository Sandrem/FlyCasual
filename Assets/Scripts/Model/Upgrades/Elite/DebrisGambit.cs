using Upgrade;
using UnityEngine;
using Ship;
using System;
using SubPhases;
using System.Linq;
using Abilities;
using Tokens;
using RuleSets;
using ActionsList;
using Obstacles;
using BoardTools;

namespace UpgradesList
{
    public class DebrisGambit : GenericUpgrade, ISecondEditionUpgrade
    {
        public DebrisGambit() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Debris Gambit";
            Cost = 2;

            UpgradeRuleType = typeof(SecondEdition);

            SEImageNumber = 3;

            UpgradeAbilities.Add(new Abilities.SecondEdition.DebrisGambitSE());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            // Not required
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Medium || ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DebrisGambitSE : GenericAbility
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
                Messages.ShowInfo(HostUpgrade.Name + ": Action is treated as white");
                return true;
            }

            foreach (GenericObstacle obstacle in ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance shipObstacleDist = new ShipObstacleDistance(HostShip, obstacle);
                if (shipObstacleDist.Range < 2)
                {
                    Messages.ShowInfo(HostUpgrade.Name + ": Action is treated as white");
                    return true;
                }
            }

            return false;
        }
    }
}
