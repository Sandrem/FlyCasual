using Ship;
using Upgrade;
using Abilities.SecondEdition;
using RuleSets;
using System;
using System.Collections.Generic;
using Obstacles;
using System.Linq;

namespace UpgradesList
{
    public class CollisionDetector : GenericUpgrade, ISecondEditionUpgrade
    {
        public CollisionDetector() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Collision Detector";
            Cost = 5;

            UsesCharges = true;
            MaxCharges = 2;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new CollisionDetectorAbilitySE());

            SEImageNumber = 24;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CollisionDetectorAbilitySE : GenericAbility
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
            if (HostShip.IsHitObstacles && HostUpgrade.Charges > 0)
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
            if (HostUpgrade.Charges > 0)
            {
                AskToUseAbility(
                    AlwaysUseByDefault,
                    TurnOnIgnoreObstacle,
                    DontIgnoreObstacle,
                    infoText: HostUpgrade.NameOriginal + ": Do you want ignore " + ObstaclesHit.First().Name,
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
            HostUpgrade.SpendCharge();

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