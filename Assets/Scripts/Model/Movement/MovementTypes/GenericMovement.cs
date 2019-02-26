using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Editions;
using Obstacles;

namespace Movement
{ 
    public abstract class GenericMovement
    {
        public bool IsRevealDial = true;
        public string GrantedBy;
        public bool IsIonManeuver;

        public ManeuverSpeed ManeuverSpeed { get; set; }
        public int Speed { get; set; }
        public ManeuverDirection Direction { get; set; }
        public ManeuverBearing Bearing { get; set; }
        public MovementComplexity ColorComplexity { get; set; }

        private Ship.GenericShip theShip;
        public Ship.GenericShip TheShip {
            get {
                return theShip ?? Selection.ThisShip;
            }
            set {
                theShip = value;
            }
        }

        public bool IsBasicManeuver
        {
            get
            {
                return Bearing == ManeuverBearing.Straight || Bearing == ManeuverBearing.Bank || Bearing == ManeuverBearing.Turn;
            }
        }

        public bool IsAdvancedManeuver
        {
            get { return !IsBasicManeuver; }
        }

        protected float ProgressTarget { get; set; }
        protected float ProgressCurrent { get; set; }

        protected float AnimationSpeed { get; set; }

        public MovementPrediction movementPrediction;
        public bool IsSimple;

        public int RotationEndDegrees = 0;

        public GenericMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color)
        {
            Speed = speed;
            ManeuverSpeed = GetManeuverSpeed(speed);
            Direction = direction;
            Bearing = bearing;
            ColorComplexity = color;
        }

        public virtual void Initialize()
        {
            ProgressTarget = SetProgressTarget();
            AnimationSpeed = Options.ManeuverSpeed * SetAnimationSpeed();
        }

        protected virtual float SetProgressTarget() { return 0; }

        protected virtual float SetAnimationSpeed() { return 0; }

        private ManeuverSpeed GetManeuverSpeed(int speed)
        {
            ManeuverSpeed maneuverSpeed = ManeuverSpeed.Speed1;

            switch (speed)
            {
                case 0:
                    maneuverSpeed = ManeuverSpeed.Speed0;
                    break;
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
            ProgressCurrent = 0f;
        }

        public virtual void LaunchShipMovement()
        {
            GameManagerScript.Wait(0.5f, delegate { TheShip.StartMoving(LaunchShipMovementContinue); });
        }

        private void LaunchShipMovementContinue()
        {
            if (ProgressTarget > 0) Rules.Collision.ClearBumps(TheShip);

            foreach (var shipBumped in movementPrediction.ShipsBumped)
            {
                Rules.Collision.AddBump(TheShip, shipBumped);
            }

            TheShip.LandedOnObstacles = new List<GenericObstacle>(movementPrediction.LandedOnObstacles);

            if (movementPrediction.AsteroidsHit.Count > 0)
            {
                TheShip.ObstaclesHit.AddRange(movementPrediction.AsteroidsHit);
            }

            if (movementPrediction.MinesHit.Count > 0)
            {
                TheShip.MinesHit.AddRange(movementPrediction.MinesHit);
            }

            Sounds.PlayFly(TheShip);
            AdaptSuccessProgress();

            LaunchSimple();
        }

        public void LaunchSimple()
        {
            //TEMP
            if (ProgressTarget > 0)
            {
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Movement.isMoving = true;
            }
            else
            {
                FinishMovement();
            }
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
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = false;

            TheShip.ApplyRotationHelpers();
            TheShip.ResetRotationHelpers();

            // TODO: Use Selection.ActiveShip instead of TheShip
            Selection.ActiveShip = TheShip;

            ManeuverEndRotation(FinishManeuverExecution);
        }

        private void FinishManeuverExecution()
        {
            MovementTemplates.HideLastMovementRuler();

            Selection.ActiveShip.CallExecuteMoving(Triggers.FinishTrigger);
        }

        protected virtual void ManeuverEndRotation(Action callBack)
        {
            callBack();
        }

        //TODO: Rework
        protected float GetMovement1()
        {
            float result = BoardTools.Board.GetBoard().TransformVector(new Vector3(4, 0, 0)).x;
            return result;
        }

        public abstract GameObject[] PlanMovement();

        public override string ToString()
        {
            string maneuverString = "";

            maneuverString += Speed + ".";

            switch (Direction)
            {
                case ManeuverDirection.Left:
                    maneuverString += "L.";
                    break;
                case ManeuverDirection.Forward:
                    maneuverString += "F.";
                    break;
                case ManeuverDirection.Right:
                    maneuverString += "R.";
                    break;
                case ManeuverDirection.Stationary:
                    maneuverString += "S.";
                    break;
                default:
                    break;
            }

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Bank:
                    maneuverString += "B";
                    break;
                case ManeuverBearing.Turn:
                    maneuverString += "T";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoopUsingTurnTemplate:
                    maneuverString += "r";
                    break;
                case ManeuverBearing.TallonRoll:
                    maneuverString += "E";
                    break;
                case ManeuverBearing.Stationary:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Reverse:
                    maneuverString += "V";
                    break;
                default:
                    break;
            }

            return maneuverString;
        }

        public string GetBearingChar()
        {
            string result = "";

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    result = "8";
                    break;
                case ManeuverBearing.Bank:
                    result = (Direction == ManeuverDirection.Left) ? "7" : "9";
                    break;
                case ManeuverBearing.Turn:
                    result = (Direction == ManeuverDirection.Left) ? "4" : "6";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    result = "2";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    result = (Direction == ManeuverDirection.Left) ? "1" : "3";
                    break;
                case ManeuverBearing.SegnorsLoopUsingTurnTemplate:
                    result = (Direction == ManeuverDirection.Left) ? "1" : "3";
                    break;
                case ManeuverBearing.TallonRoll:
                    result = (Direction == ManeuverDirection.Left) ? ":" : ";";
                    break;
                case ManeuverBearing.Stationary:
                    result = "5";
                    break;
                case ManeuverBearing.Reverse:
                    switch (Direction)
                    {
                        case ManeuverDirection.Left:
                            result = "J";
                            break;
                        case ManeuverDirection.Forward:
                            result = "K";
                            break;
                        case ManeuverDirection.Right:
                            result = "L";
                            break;
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public Color GetColor()
        {
            Color result = Color.yellow;
            switch (ColorComplexity)
            {
                case MovementComplexity.None:
                    break;
                case MovementComplexity.Easy:
                    result = Edition.Current.MovementEasyColor;
                    break;
                case MovementComplexity.Normal:
                    result = Color.white;
                    break;
                case MovementComplexity.Complex:
                    result = Color.red;
                    break;
                default:
                    break;
            }
            return result;
        }

        public static MovementComplexity ReduceComplexity(MovementComplexity complexity)
        {
            switch (complexity)
            {
                case MovementComplexity.Normal:
                    complexity = MovementComplexity.Easy;
                    break;
                case MovementComplexity.Complex:
                    complexity = MovementComplexity.Normal;
                    break;
            }

            return complexity;
        }

        public static MovementComplexity IncreaseComplexity(MovementComplexity complexity)
        {
            switch (complexity)
            {
                case MovementComplexity.Normal:
                    complexity = MovementComplexity.Complex;
                    break;
                case MovementComplexity.Easy:
                    complexity = MovementComplexity.Normal;
                    break;
            }

            return complexity;
        }

        public static List<string> GetAllManeuvers()
        {
            List<string> result = new List<string>();

            result.Add("2.F.V");

            result.Add("1.F.V");
            result.Add("1.L.V");
            result.Add("1.R.V");

            result.Add("0.S.S");

            result.Add("1.L.T");
            result.Add("2.L.T");
            result.Add("3.L.T");

            result.Add("1.L.B");
            result.Add("2.L.B");
            result.Add("3.L.B");

            result.Add("1.F.S");
            result.Add("2.F.S");
            result.Add("3.F.S");
            result.Add("4.F.S");
            result.Add("5.F.S");

            result.Add("1.R.B");
            result.Add("2.R.B");
            result.Add("3.R.B");

            result.Add("1.R.T");
            result.Add("2.R.T");
            result.Add("3.R.T");

            result.Add("1.L.R");
            result.Add("2.L.R");
            result.Add("3.L.R");

            result.Add("1.L.E");
            result.Add("2.L.E");
            result.Add("3.L.E");

            result.Add("1.F.R");
            result.Add("2.F.R");
            result.Add("3.F.R");
            result.Add("4.F.R");
            result.Add("5.F.R");

            result.Add("1.R.E");
            result.Add("2.R.E");
            result.Add("3.R.E");

            result.Add("1.R.R");
            result.Add("2.R.R");
            result.Add("3.R.R");

            return result;
        }

    }

}

