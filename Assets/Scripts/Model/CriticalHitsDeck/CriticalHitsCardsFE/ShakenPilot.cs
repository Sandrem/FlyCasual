using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class ShakenPilot : GenericDamageCard
    {
        public ShakenPilot()
        {
            Name = "Shaken Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverAvailablity += CannotBeAssignedStraightManeuvers;
            Host.OnMovementFinish += CallDiscardEffect;

            Host.Tokens.AssignToken(new Tokens.ShakenPilotCritToken(Host), Triggers.FinishTrigger);
        }

        private void CallDiscardEffect(GenericShip ship)
        {
            DiscardEffect();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Can be assigned straight maneuvers");
            Host.Tokens.RemoveCondition(typeof(Tokens.ShakenPilotCritToken));

            Host.AfterGetManeuverAvailablity -= CannotBeAssignedStraightManeuvers;
            Host.OnMovementFinish -= CallDiscardEffect;
        }

        private void CannotBeAssignedStraightManeuvers(GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.Bearing == Movement.ManeuverBearing.Straight)
            {
                //Too many
                //Game.UI.ShowError("Shaken Pilot: Cannot be assigned straight maneuvers");
                //Game.UI.AddTestLogEntry("Shaken Pilot: Cannot be assigned straight maneuvers");
                movement.ColorComplexity = Movement.MovementComplexity.None;
            }
        }

    }

}