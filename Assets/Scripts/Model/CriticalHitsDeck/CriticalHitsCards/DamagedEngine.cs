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
            Messages.ShowInfo("Treat all turn maneuvers as red maneuvers");
            Game.UI.AddTestLogEntry("Treat all turn maneuvers as red maneuvers");

            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreRed;

            Host.AssignToken(new Tokens.DamagedEngineCritToken(), Triggers.FinishTrigger);
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Turn maneuvers regained normal colors");
            Game.UI.AddTestLogEntry("Turn maneuvers regained normal colors");
            host.RemoveToken(typeof(Tokens.DamagedEngineCritToken));

            host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreRed;
        }

        private void TurnManeuversAreRed(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.ManeuverColor.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.Red;
                }
            }
        }

    }

}