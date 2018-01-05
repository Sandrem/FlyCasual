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
            HostShip.OnMovementFinish += TargetingAstromechTargetLock;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= TargetingAstromechTargetLock;
        }

        private void TargetingAstromechTargetLock(GenericShip hostShip)
        {
            Movement.ManeuverColor movementColor = hostShip.GetLastManeuverColor();
            if (movementColor != Movement.ManeuverColor.Red)
            {                
                return;
            }

            if (Actions.HasTarget(hostShip))
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuver, SelectTargetForLock);
            }
        }

        public void SelectTargetForLock(object sender, EventArgs e)
        {
            SelectTargetForAbility(AssignAstromechTargetingLock, new List<TargetTypes>() { TargetTypes.Enemy }, new Vector2(1, 3), null, true);            
        }

        private void AssignAstromechTargetingLock()
        {
            Actions.AssignTargetLockToPair(
                HostShip,
                TargetShip,
                delegate
                {
                    UI.HideSkipButton();
                    Phases.FinishSubPhase(typeof(SelectTargetLockSubPhase));
                    DecisionSubPhase.ConfirmDecision();
                },
                DecisionSubPhase.ConfirmDecision);
        }
    }
}