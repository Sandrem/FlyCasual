using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class BankBoost : BankMovement
    {
        private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

        public BankBoost(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
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

            Phases.FinishSubPhase(typeof(SubPhases.BoostExecutionSubPhase));
            Triggers.FinishTrigger();
        }
    }

}

