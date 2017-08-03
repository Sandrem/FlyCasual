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
            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R2-D2";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/5/57/R2-d2.png";
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
                    Triggers.RegisterTrigger(new Trigger() { Name = "R2-D2: Regen Shield", TriggerOwner = host.Owner.PlayerNo, triggerType = TriggerTypes.OnShipMovementExecuted, eventHandler = R2D2RegenShield });
                }
            }
        }

        private void R2D2RegenShield(object sender, EventArgs e)
        {
            if (Host.TryRegenShields())
            {
                Sounds.PlaySoundOnce("R2D2-Proud");
                Game.UI.ShowInfo("R2-D2: Shield is restored");
            }
            Triggers.FinishTrigger();
        }

    }

}
