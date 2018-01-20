﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class TurnBoost : TurnMovement
    {
        public TurnBoost(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {
        }

        public override void Perform()
        {
            Initialize();

            //Temporary
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateBoost);
        }

        private bool UpdateBoost()
        {
            UpdateMovementExecution();
            return false;
        }

        protected override void FinishMovement()
        {
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Remove(UpdateBoost);

            MovementTemplates.HideLastMovementRuler();
            Selection.ThisShip.ResetRotationHelpers();

            (Phases.CurrentSubPhase as SubPhases.BoostExecutionSubPhase).FinishBoost();
        }
    }
}
