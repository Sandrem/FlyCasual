using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class ShakenPilot : GenericCriticalHit
    {
        public ShakenPilot()
        {
            Name = "Shaken Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverAvailablity += CannotBeAssignedStraightManeuvers;
            Host.OnMovementFinish += DiscardEffect;

            Host.AssignToken(new Tokens.ShakenPilotCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Can be assigned straight maneuvers");
            host.RemoveToken(typeof(Tokens.ShakenPilotCritToken));

            host.AfterGetManeuverAvailablity -= CannotBeAssignedStraightManeuvers;
            Host.OnMovementFinish -= DiscardEffect;
        }

        private void CannotBeAssignedStraightManeuvers(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.Bearing == Movement.ManeuverBearing.Straight)
            {
                //Too many
                //Game.UI.ShowError("Shaken Pilot: Cannot be assigned straight maneuvers");
                //Game.UI.AddTestLogEntry("Shaken Pilot: Cannot be assigned straight maneuvers");
                movement.ColorComplexity = Movement.ManeuverColor.None;
            }
        }

    }

}