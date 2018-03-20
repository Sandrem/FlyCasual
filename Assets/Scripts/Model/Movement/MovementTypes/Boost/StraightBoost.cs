using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class StraightBoost : StraightMovement
    {
        public StraightBoost(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            ProgressTarget = SetProgressTarget();
            AnimationSpeed = Options.ManeuverSpeed * SetAnimationSpeed();

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
            TheShip.ResetRotationHelpers();

            (Phases.CurrentSubPhase as SubPhases.BoostExecutionSubPhase).FinishBoost();
        }

    }

}

