using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{

    public class K4SecurityDroid : GenericUpgrade
    {
        // This is used to set the Action Name, so user can distinguish what Free Action they are performing
        private string ActionName;

        public K4SecurityDroid() : base()
        {
            Type = UpgradeType.Crew;
            ActionName = "K4 Security Droid: Target Lock";
            Name = "K4 Security Droid";
            Cost = 4;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnMovementFinish += PlanTargetLock;
        }

        private void PlanTargetLock(Ship.GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                /*
                     This uses OnMovementFinish instead of OnMovementExecuted so user doesn't have to make decision
                     between resolving "Check Stress" (which happens OnMovementExecuted) and "K4 Security Droid: Target Lock".
                     That made for a weird situation where a ship with a stress token could choose to use K4 Security Droid before
                     Check Stress, which would cause K4 Security Droid to fail.
                */
                Triggers.RegisterTrigger(new Trigger() {
                    Name = ActionName,
                    TriggerOwner = host.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnShipMovementFinish,
                    EventHandler = PerformTargetLock
                });
            }
        }

        private void PerformTargetLock(object sender, EventArgs e)
        {
            ActionsList.TargetLockAction K4SecurityDroidTargetLock = new ActionsList.TargetLockAction()
            {
                Name = ActionName,
                EffectName = ActionName
            };

            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction> { K4SecurityDroidTargetLock };
            base.Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
