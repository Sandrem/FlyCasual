using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCard
{

    public class LooseStabilizer : GenericDamageCard
    {
        public LooseStabilizer()
        {
            Name = "Loose Stabilizer";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.OnMovementFinish += PlanStressAfterWhiteManeuvers;
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;

            Host.Tokens.AssignCondition(new Tokens.LooseStabilizerCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("No stress after white maneuvers");
            Host.Tokens.RemoveCondition(typeof(Tokens.LooseStabilizerCritToken));
            Host.OnMovementFinish -= PlanStressAfterWhiteManeuvers;
            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
        }

        private void PlanStressAfterWhiteManeuvers(GenericShip ship)
        {
            if (Host.GetLastManeuverColor() == Movement.ManeuverColor.White)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Loose Stabilizer: Stress after white maneuver",
                    TriggerOwner = Host.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnShipMovementExecuted,
                    EventHandler = StressAfterWhiteManeuvers
                });
            }
        }

        private void StressAfterWhiteManeuvers(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Loose Stabilizer: Stress token is assigned");
            UI.AddTestLogEntry("Loose Stabilizer: Stress token is assigned");
            Selection.ThisShip.Tokens.AssignToken(new Tokens.StressToken(Host), Triggers.FinishTrigger);
        }

    }

}