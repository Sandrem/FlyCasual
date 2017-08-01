using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public enum ManeuverSpeed
    {
        AdditionalMovement,
        Speed1,
        Speed2,
        Speed3,
        Speed4,
        Speed5
    }

    public enum ManeuverDirection
    {
        Left,
        Forward,
        Right
    }

    public enum ManeuverBearing
    {
        Straight,
        Bank,
        Turn,
        KoiogranTurn
    }

    public enum ManeuverColor
    {
        None,
        Green,
        White,
        Red
    }

    public struct MovementStruct
    {
        public ManeuverSpeed Speed;
        public ManeuverDirection Direction;
        public ManeuverBearing Bearing;
        public ManeuverColor ColorComplexity;
    }

    public abstract class GenericMovement
    {
        public ManeuverSpeed ManeuverSpeed { get; set; }
        public int Speed { get; set; }
        public ManeuverDirection Direction { get; set; }
        public ManeuverBearing Bearing { get; set; }
        public ManeuverColor ColorComplexity { get; set; }

        protected float ProgressTarget { get; set; }
        protected float ProgressCurrent { get; set; }

        protected float AnimationSpeed { get; set; }

        protected MovementPrediction movementPrediction;

        public GenericMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color)
        {
            Speed = speed;
            ManeuverSpeed = GetManeuverSpeed(speed);
            Direction = direction;
            Bearing = bearing;
            ColorComplexity = color;
        }

        protected virtual void Initialize()
        {
            ProgressTarget = SetProgressTarget();
            AnimationSpeed = SetAnimationSpeed();
        }

        protected virtual float SetProgressTarget() { return 0; }

        protected virtual float SetAnimationSpeed() { return 0; }

        private ManeuverSpeed GetManeuverSpeed(int speed)
        {
            ManeuverSpeed maneuverSpeed = ManeuverSpeed.Speed1;

            switch (speed)
            {
                case 1:
                    maneuverSpeed = ManeuverSpeed.Speed1;
                    break;
                case 2:
                    maneuverSpeed = ManeuverSpeed.Speed2;
                    break;
                case 3:
                    maneuverSpeed = ManeuverSpeed.Speed3;
                    break;
                case 4:
                    maneuverSpeed = ManeuverSpeed.Speed4;
                    break;
                case 5:
                    maneuverSpeed = ManeuverSpeed.Speed5;
                    break;
            }

            return maneuverSpeed;
        }

        public virtual void Perform()
        {
            Selection.ThisShip.StartMoving();
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.UI.HideContextMenu();
            ProgressCurrent = 0f;
        }

        public virtual void LaunchShipMovement()
        {

            //Move to method - finish of movement prediction
            Selection.ThisShip.ShipsBumped.AddRange(movementPrediction.ShipsBumped);
            foreach (var bumpedShip in Selection.ThisShip.ShipsBumped)
            {
                if (!bumpedShip.ShipsBumped.Contains(Selection.ThisShip))
                {
                    bumpedShip.ShipsBumped.Add(Selection.ThisShip);
                }
            }

            if (movementPrediction.IsLandedOnAsteroid)
            {
                Messages.ShowErrorToHuman("Landed on asteroid");
                Selection.ThisShip.IsLandedOnObstacle = movementPrediction.IsLandedOnAsteroid;
            }

            if (movementPrediction.AsteroidsHit.Count > 0)
            {
                Selection.ThisShip.ObstaclesHit.AddRange(movementPrediction.AsteroidsHit);
            }

            Sounds.PlayFly();
            AdaptSuccessProgress();

            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = true;
        }

        public virtual void UpdateMovementExecution()
        {
            CheckFinish();
        }

        public abstract void AdaptSuccessProgress();

        protected virtual void CheckFinish()
        {
            if (ProgressCurrent == ProgressTarget)
            {
                FinishMovement();
            }
        }

        protected virtual void FinishMovement()
        {
            Selection.ThisShip.IsAttackPerformed = false;

            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = false;

            MovementTemplates.HideLastMovementRuler();

            Selection.ThisShip.ResetRotationHelpers();

            Selection.ThisShip.ExecuteMoving();

            //Called as callbacks
            //Selection.ThisShip.FinishMovement();
            //Selection.ThisShip.FinishPosition();
            //Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase));
        }

        //TODO: Rework
        protected float GetMovement1()
        {
            float result = Board.GetBoard().TransformVector(new Vector3(4, 0, 0)).x;
            return result;
        }

        public abstract GameObject[] PlanMovement();

    }

}

