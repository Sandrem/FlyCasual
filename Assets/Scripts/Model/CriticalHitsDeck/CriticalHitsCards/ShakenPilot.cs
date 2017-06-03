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

        public override void ApplyEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Cannot be assigned straight maneuvers");
            Game.UI.AddTestLogEntry("Cannot be assigned straight maneuvers");
            host.AssignToken(new Tokens.ShakenPilotCritToken());

            host.AfterGetManeuverAvailablity += CannotBeAssignedStraightManeuvers;
            host.OnMovementFinish += DiscardEffect;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Can be assigned straight maneuvers");
            Game.UI.AddTestLogEntry("Can be assigned straight maneuvers");
            host.RemoveToken(typeof(Tokens.ShakenPilotCritToken));

            host.AfterGetManeuverAvailablity -= CannotBeAssignedStraightManeuvers;
        }

        private void CannotBeAssignedStraightManeuvers(Ship.GenericShip ship, ref Movement movement)
        {
            if (movement.Bearing == ManeuverBearing.Straight)
            {
                //Too many
                //Game.UI.ShowError("Shaken Pilot: Cannot be assigned straight maneuvers");
                //Game.UI.AddTestLogEntry("Shaken Pilot: Cannot be assigned straight maneuvers");
                movement.ColorComplexity = Ship.ManeuverColor.None;
            }
        }

    }

}