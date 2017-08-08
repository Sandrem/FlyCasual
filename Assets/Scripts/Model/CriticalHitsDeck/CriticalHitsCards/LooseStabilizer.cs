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
            ImageUrl = "http://i.imgur.com/Kouy0v8.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("After you execute a white maneuver, receive 1 stress token");
            Game.UI.AddTestLogEntry("After you execute a white maneuver, receive 1 stress token");
            Host.AssignToken(new Tokens.LooseStabilizerCritToken());

            Host.OnMovementFinish += StressAfterWhiteManeuvers;
            Host.AfterGenerateAvailableActionsList += AddCancelCritAction;

            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("No stress after white maneuvers");
            Game.UI.AddTestLogEntry("No stress after white maneuvers");
            host.RemoveToken(typeof(Tokens.LooseStabilizerCritToken));

            host.OnMovementFinish -= StressAfterWhiteManeuvers;
            host.AfterGenerateAvailableActionsList -= AddCancelCritAction;
        }

        private void StressAfterWhiteManeuvers(Ship.GenericShip ship)
        {
            if (ship.GetLastManeuverColor() == Movement.ManeuverColor.White)
            {
                Game.UI.ShowError("Loose Stabilizer: Stress token is assigned");
                Game.UI.AddTestLogEntry("Loose Stabilizer: Stress token is assigned");
                ship.AssignToken(new Tokens.StressToken());
            }
        }

    }

}