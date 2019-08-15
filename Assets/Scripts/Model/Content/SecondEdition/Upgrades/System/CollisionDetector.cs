﻿using Ship;
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
                cost: 6,
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

            HostShip.OnPositionIsReadyToFinish += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.IsIgnoreObstaclesDuringBoost = false;
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = false;

            HostShip.OnPositionIsReadyToFinish -= TryRegisterAbility;
        }

        private void TryRegisterAbility(GenericShip ship)
        {
            if (HostShip.IsHitObstacles && HostUpgrade.State.Charges > 0)
            {
                foreach (GenericObstacle obstacle in HostShip.ObstaclesHit)
                {
                    ObstaclesHit.Add(obstacle);
                    RegisterAbilityTrigger(TriggerTypes.OnPositionIsReadyToFinish, ActivateCollisionDetectorAbility);
                }
            }
        }

        private void ActivateCollisionDetectorAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    TurnOnIgnoreObstacle,
                    DontIgnoreObstacle,
                    descriptionLong: "Do you want to spend 1 Charge to ignore ignore effect of " + ObstaclesHit.First().Name + " until the end of the round?",
                    imageHolder: HostUpgrade,
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