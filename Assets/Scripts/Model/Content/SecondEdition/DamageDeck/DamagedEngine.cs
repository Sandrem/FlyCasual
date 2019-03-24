using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{

    public class DamagedEngine : GenericDamageCard
    {
        public DamagedEngine()
        {
            Name = "Damaged Engine";
            Type = CriticalCardType.Ship;
            ImageUrl = "https://i.imgur.com/1sYXXBQ.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreHarder;

            Host.Tokens.AssignCondition(typeof(Tokens.DamagedEngineSECritToken));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Messages.ShowInfo("Damaged Engine has been repaired.  " + Host.PilotInfo.PilotName + "'s turn maneuvers regained normal colors.");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedEngineSECritToken));

            Host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreHarder;
        }

        private void TurnManeuversAreHarder(Ship.GenericShip ship, ref Movement.ManeuverHolder movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Turn)
                {
                    movement.ColorComplexity = Movement.GenericMovement.IncreaseComplexity(movement.ColorComplexity);
                }
            }
        }
    }
}