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
            ImageUrl = "https://i.imgur.com/sBDC3iQ.jpg";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreHarder;

            Host.Tokens.AssignCondition(new Tokens.DamagedEngineSECritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect()
        {
            Messages.ShowInfo("Turn maneuvers regained normal colors");
            Host.Tokens.RemoveCondition(typeof(Tokens.DamagedEngineSECritToken));

            Host.AfterGetManeuverColorIncreaseComplexity -= TurnManeuversAreHarder;
        }

        private void TurnManeuversAreHarder(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.MovementComplexity.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Turn)
                {
                    switch(movement.ColorComplexity)
                    {
                        case Movement.MovementComplexity.Easy:
                            movement.ColorComplexity = Movement.MovementComplexity.Normal;
                            break;
                        case Movement.MovementComplexity.Normal:
                            movement.ColorComplexity = Movement.MovementComplexity.Complex;
                            break;
                    }                    
                }
            }
        }
    }
}