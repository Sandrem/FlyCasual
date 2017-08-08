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
            ImageUrl = "http://i.imgur.com/BGMZR5Q.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("Cannot be assigned straight maneuvers");
            Game.UI.AddTestLogEntry("Cannot be assigned straight maneuvers");
            Host.AssignToken(new Tokens.ShakenPilotCritToken());

            Host.AfterGetManeuverAvailablity += CannotBeAssignedStraightManeuvers;
            Host.OnMovementFinish += DiscardEffect;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Can be assigned straight maneuvers");
            Game.UI.AddTestLogEntry("Can be assigned straight maneuvers");
            host.RemoveToken(typeof(Tokens.ShakenPilotCritToken));

            host.AfterGetManeuverAvailablity -= CannotBeAssignedStraightManeuvers;
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