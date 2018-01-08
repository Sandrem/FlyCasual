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
            Movement.ManeuverColor movementColor = HostShip.GetLastManeuverColor();
            if (movementColor != Movement.ManeuverColor.Red)
            {                
                return;
            }            

            if (Actions.HasTarget(HostShip))
            {
                AssignAstromechTargetingLock(HostShip);
            }
        }

        private void AssignAstromechTargetingLock(GenericShip hostShip)
        {
            hostShip.AcquireTargetLock(Phases.CurrentSubPhase.Next);
        }
    }
}