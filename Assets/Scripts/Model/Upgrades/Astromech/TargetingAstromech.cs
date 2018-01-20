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
            HostShip.OnMovementFinish += RegisterTargetingAstromech;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTargetingAstromech;
        }

        private void RegisterTargetingAstromech(GenericShip hostShip)
        {
            Movement.ManeuverColor movementColor = HostShip.GetLastManeuverColor();
            if (movementColor != Movement.ManeuverColor.Red)
            {
                return;
            }

            RegisterAbilityTrigger(
                TriggerTypes.OnShipMovementFinish, 
                AssignAstromechTargetingLock
            );            
        }

        private void AssignAstromechTargetingLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Targeting Astromech: Aquire a Target Lock");
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            HostShip.AcquireTargetLock(Triggers.FinishTrigger);
        }
    }
}