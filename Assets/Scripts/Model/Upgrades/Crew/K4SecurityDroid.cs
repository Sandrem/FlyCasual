using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{

    public class K4SecurityDroid : GenericUpgrade
    {
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
