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

    public class GenericMovement
    {
        public ManeuverSpeed ManeuverSpeed { get; set; }
        public int Speed { get; set; }
        public ManeuverDirection Direction { get; set; }
        public ManeuverBearing Bearing { get; set; }
        public ManeuverColor ColorComplexity { get; set; }

        protected float ProgressTarget { get; set; }
        protected float ProgressCurrent { get; set; }

        protected float AnimationSpeed { get; set; }

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
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.UI.HideContextMenu();
            Sounds.PlayFly();
            ProgressCurrent = 0f;
        }

        public virtual void LaunchShipMovement()
        {
            Selection.ThisShip.StartMoving();

            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = true;
        }

        public virtual void UpdateMovementExecution()
        {
            CheckFinish();
        }

        protected virtual void CheckFinish()
        {
            if (ProgressCurrent == ProgressTarget)
            {
                FinishMovementWithoutColliding();
            }
        }

        protected virtual void FinishMovementWithoutColliding()
        {
            Selection.ThisShip.FinishMovementWithoutColliding();

            MovementTemplates.HideLastMovementRuler();

            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = false;

            Game.StartCoroutine(FinishMovementCoroutine());
        }

        protected IEnumerator FinishMovementCoroutine()
        {
            yield return Selection.ThisShip.ExecuteMoving();

            // Selection.ThisShip.CheckLandedOnObstacle();

            yield return Selection.ThisShip.FinishMoving();

            Selection.ThisShip.FinishPosition();

            Selection.ThisShip.ResetRotationHelpers();

            Selection.ThisShip.IsAttackPerformed = false;

            Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase));
        }

        //TODO: Rework
        protected float GetMovement1()
        {
            float result = Board.GetBoard().TransformVector(new Vector3(4, 0, 0)).x;
            return result;
        }
    }

}

