using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R2D2 : GenericUpgrade
    {

        public R2D2() : base()
        {
            Type = UpgradeType.Astromech;
            Name = ShortName = "R2-D2";
            isUnique = true;
            Cost = 4;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnMovementExecuted += R2D2PlanRegenShield;
        }

        private void R2D2PlanRegenShield(Ship.GenericShip host)
        {
            if (host.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
            {
                if (host.Shields < host.MaxShields)
                {
                    Triggers.RegisterTrigger(new Trigger() {
                        Name = "R2-D2: Regen Shield",
                        TriggerOwner = host.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnShipMovementExecuted,
                        EventHandler = R2D2RegenShield
                    });
                }
            }
        }

        private void R2D2RegenShield(object sender, EventArgs e)
        {
            if (Host.TryRegenShields())
            {
                Sounds.PlayShipSound("R2D2-Proud");
                Messages.ShowInfo("R2-D2: Shield is restored");
            }
            Triggers.FinishTrigger();
        }

    }

}
