using Ship;
using Upgrade;
using ActionsList;
using System;
using System.Collections.Generic;
using Obstacles;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class CollisionDetector : GenericUpgrade
    {
        public CollisionDetector() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Collision Detector",
                UpgradeType.System,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.CollisionDetectorAbility),
                charges: 2,
                seImageNumber: 24
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CollisionDetectorAbility : GenericAbility
    {
        List<GenericObstacle> ObstaclesHit = new List<GenericObstacle>();
        List<GenericObstacle> IgnoredObstacles = new List<GenericObstacle>();

        public override void ActivateAbility()
        {
            HostShip.IsIgnoreObstaclesDuringBoost = true;
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = true;

            HostShip.OnMovementExecuted += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.IsIgnoreObstaclesDuringBoost = false;
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = false;

            HostShip.OnMovementExecuted -= TryRegisterAbility;
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            if (HostShip.IsHitObstacles && HostUpgrade.State.Charges > 0)
            {
                foreach (GenericObstacle obstacle in HostShip.ObstaclesHit)
                {
                    ObstaclesHit.Add(obstacle);
                    RegisterAbilityTrigger(TriggerTypes.OnMovementExecuted, ActivateCollisionDetectorAbility);
                }
            }
        }

        private void ActivateCollisionDetectorAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                AskToUseAbility(
                    AlwaysUseByDefault,
                    TurnOnIgnoreObstacle,
                    DontIgnoreObstacle,
                    infoText: HostUpgrade.UpgradeInfo.Name + ": Do you want ignore " + ObstaclesHit.First().Name,
                    showSkipButton: false
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DontIgnoreObstacle(object sender, EventArgs e)
        {
            ObstaclesHit.Remove(ObstaclesHit.First());
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void TurnOnIgnoreObstacle(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();

            HostShip.IgnoreObstaclesList.Add(ObstaclesHit.First());
            IgnoredObstacles.Add(ObstaclesHit.First());
            Phases.Events.OnRoundEnd += ClearAbility;

            ObstaclesHit.Remove(ObstaclesHit.First());
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void ClearAbility()
        {
            Phases.Events.OnRoundEnd -= ClearAbility;

            HostShip.IgnoreObstaclesList.Remove(IgnoredObstacles.First());
            IgnoredObstacles.Remove(IgnoredObstacles.First());
        }
    }
}