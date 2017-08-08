using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriticalHitCard
{

    public class DamagedEngine : GenericCriticalHit
    {
        public DamagedEngine()
        {
            Name = "Damaged Engine";
            Type = CriticalCardType.Ship;
            ImageUrl = "http://i.imgur.com/BvKig48.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Game.UI.ShowInfo("Treat all turn maneuvers as red maneuvers");
            Game.UI.AddTestLogEntry("Treat all turn maneuvers as red maneuvers");
            Host.AssignToken(new Tokens.DamagedEngineCritToken());

            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreRed;
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Game.UI.ShowInfo("Turn maneuvers regained normal colors");
            Game.UI.AddTestLogEntry("Turn maneuvers regained normal colors");
            host.RemoveToken(typeof(Tokens.DamagedEngineCritToken));

            host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreRed;
        }

        private void TurnManeuversAreRed(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.Bearing == Movement.ManeuverBearing.Turn)
            {
                //Too many notifications
                //Game.UI.ShowInfo("Damaged Engine: Treat all turn maneuvers as red maneuvers");
                //Game.UI.AddTestLogEntry("Damaged Engine: Treat all turn maneuvers as red maneuvers");
                movement.ColorComplexity = Movement.ManeuverColor.Red;
            }
        }

    }

}