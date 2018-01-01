using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class LooseStabilizer : GenericCriticalHit
    {
        public LooseStabilizer()
        {
            Name = "Loose Stabilizer";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += PlanStressAfterWhiteManeuvers;
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Host.AssignToken(new Tokens.LooseStabilizerCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("No stress after white maneuvers");
            host.RemoveToken(typeof(Tokens.LooseStabilizerCritToken));
            host.OnMovementFinish -= PlanStressAfterWhiteManeuvers;
            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void PlanStressAfterWhiteManeuvers(Ship.GenericShip ship)
        {
            if (Selection.ThisShip.GetLastManeuverColor() == Movement.ManeuverColor.White)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Loose Stabilizer: Stress after white maneuver",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnShipMovementExecuted,
                    EventHandler = StressAfterWhiteManeuvers
                });
            }
        }

        private void StressAfterWhiteManeuvers(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Loose Stabilizer: Stress token is assigned");
            UI.AddTestLogEntry("Loose Stabilizer: Stress token is assigned");
            Selection.ThisShip.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
        }

    }

}