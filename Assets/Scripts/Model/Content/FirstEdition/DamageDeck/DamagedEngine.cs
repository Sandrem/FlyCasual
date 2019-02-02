using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardFE
{

    public class DamagedEngine : GenericDamageCard
    {
        public DamagedEngine()
        {
            Name = "Damaged Engine";
            Type = CriticalCardType.Ship;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreRed;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedEngineCritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Turn maneuvers regained normal colors");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedEngineCritToken));

            Host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreRed;
        }

        private void TurnManeuversAreRed(Ship.GenericShip ship, ref Movement.ManeuverHolder movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = Movement.MovementComplexity.Complex;
                }
            }
        }

    }

}