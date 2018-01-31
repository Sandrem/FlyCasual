﻿using System;
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
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {
            Host.AfterGetManeuverColorIncreaseComplexity += TurnManeuversAreRed;

            Host.Tokens.AssignCondition(new Tokens.DamagedEngineCritToken(Host));
            Triggers.FinishTrigger();
        }

        public override void DiscardEffect(Ship.GenericShip host)
        {
            Messages.ShowInfo("Turn maneuvers regained normal colors");
            host.Tokens.RemoveCondition(typeof(Tokens.DamagedEngineCritToken));

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