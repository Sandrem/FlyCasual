using Abilities;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class TargetingAstromech : GenericUpgrade
    {

        public TargetingAstromech() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "Targeting Astromech";
            Cost = 2;

            UpgradeAbilities.Add(new TargetingAstromechAbility());
        }
    }
}

namespace Abilities
{
    public class TargetingAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementExecuted += RegisterTargetingAstromech;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementExecuted -= RegisterTargetingAstromech;
        }

        private void RegisterTargetingAstromech(GenericShip hostShip)
        {
            Movement.ManeuverColor movementColor = HostShip.GetLastManeuverColor();
            if (movementColor != Movement.ManeuverColor.Red)
            {
                return;
            }

            RegisterAbilityTrigger(
                TriggerTypes.OnManeuver, 
                delegate 
                {
                    AssignAstromechTargetingLock(hostShip);                    
                });            
        }

        private void AssignAstromechTargetingLock(GenericShip hostShip)
        {
            hostShip.AcquireTargetLock<TargetingAstromechSubPhase>(Triggers.FinishTrigger);
        }
    }
}

namespace SubPhases
{
    public class TargetingAstromechSubPhase : SelectTargetLockSubPhase
    {
        public override void RevertSubPhase()
        {
            Triggers.FinishTrigger();
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(TargetingAstromechSubPhase));
            Triggers.FinishTrigger();
        }

        protected override void TrySelectTargetLock()
        {
            if (Rules.TargetLocks.TargetLockIsAllowed(Selection.ThisShip, TargetShip))
            {
                Actions.AssignTargetLockToPair(
                    Selection.ThisShip,
                    TargetShip,
                    delegate
                    {
                        Phases.FinishSubPhase(typeof(TargetingAstromechSubPhase));
                        Triggers.FinishTrigger();
                    },
                    RevertSubPhase
                );
            }
            else
            {
                RevertSubPhase();
            }
        }
    }
}