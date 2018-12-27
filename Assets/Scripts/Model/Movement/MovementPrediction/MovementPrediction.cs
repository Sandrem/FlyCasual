using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Obstacles;

namespace Movement
{

    public class MovementPrediction
    {
        private Action CallBack;

        private int updatesCount = 0;
        public GenericMovement CurrentMovement;
        private GameObject[] generatedShipStands;

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<Ship.GenericShip> ShipsBumped = new List<Ship.GenericShip>();
        public List<GenericObstacle> AsteroidsHit = new List<GenericObstacle>();
        public List<GameObject> MinesHit = new List<GameObject>();
        public bool IsLandedOnAsteroid { get { return LandedOnObstacles.Count > 0; } }
        public List<GenericObstacle> LandedOnObstacles = new List<GenericObstacle>();
        public float SuccessfullMovementProgress { get; private set; }
        public bool IsOffTheBoard;

        public Vector3 FinalPosition { get; private set; }
        public Vector3 FinalAngles { get; private set; }

        public MovementPrediction(GenericMovement movement, Action callBack)
        {
            CurrentMovement = movement;
            CallBack = callBack;

            Selection.ThisShip.ToggleColliders(false);
            GenerateShipStands();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        private void GenerateShipStands()
        {
            generatedShipStands = CurrentMovement.PlanMovement();
        }

        private bool UpdateColisionDetection()
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults();
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults()
        {
            bool finalPositionFound = false;
            SuccessfullMovementProgress = 0;
            ObstaclesStayDetector lastShipBumpDetector = null;

            for (int i = generatedShipStands.Length - 1; i >= 0; i--)
            {
                ObstaclesStayDetector obstacleStayDetector = generatedShipStands[i].GetComponentInChildren<ObstaclesStayDetector>();
                ObstaclesHitsDetector obstacleHitsDetector = generatedShipStands[i].GetComponentInChildren<ObstaclesHitsDetector>();

                if (!finalPositionFound)
                {
                    if (obstacleStayDetector.OverlapsShip)
                    {
                        lastShipBumpDetector = obstacleStayDetector;
                    }
                    else
                    {
                        IsOffTheBoard = obstacleStayDetector.OffTheBoard;
                        LandedOnObstacles = new List<GenericObstacle>(obstacleStayDetector.OverlapedAsteroids);
                        SuccessfullMovementProgress = (float)(i) / (generatedShipStands.Length - 1);

                        if (lastShipBumpDetector != null)
                        {
                            foreach (var overlapedShip in lastShipBumpDetector.OverlapedShips)
                            {
                                if (!ShipsBumped.Contains(overlapedShip))
                                {
                                    ShipsBumped.Add(overlapedShip);
                                }
                            }
                        }

                        foreach (var asteroidHit in obstacleStayDetector.OverlapedAsteroids)
                        {
                            if (!AsteroidsHit.Contains(asteroidHit))
                            {
                                AsteroidsHit.Add(asteroidHit);
                            }
                        }

                        foreach (var mineHit in obstacleStayDetector.OverlapedMines)
                        {
                            GameObject MineObject = mineHit.transform.parent.gameObject;
                            if (!MinesHit.Contains(MineObject))
                            {
                                MinesHit.Add(MineObject);
                            }
                        }

                        finalPositionFound = true;

                        FinalPosition = generatedShipStands[i].transform.position;
                        FinalAngles = generatedShipStands[i].transform.eulerAngles;

                        //break;
                    }
                }
                else
                {
                    foreach (GenericObstacle asteroidHit in obstacleHitsDetector.OverlapedAsteroids)
                    {
                        if (!AsteroidsHit.Contains(asteroidHit))
                        {
                            AsteroidsHit.Add(asteroidHit);
                        }
                    }
                    foreach (var mineHit in obstacleHitsDetector.OverlapedMines)
                    {
                        GameObject MineObject = mineHit.transform.parent.gameObject;
                        if (!MinesHit.Contains(MineObject))
                        {
                            MinesHit.Add(MineObject);
                        }
                    }
                }

            }

            Selection.ThisShip.ToggleColliders(true);

            if (!DebugManager.DebugMovement)
            {
                DestroyGeneratedShipStands();
                CallBack();
            }
            else
            {
                DestroyGeneratedShipStands(); CallBack();
                //GameManagerScript.Wait(2, delegate { DestroyGeneratedShipStands(); CallBack(); });
            }
        }

        private void DestroyGeneratedShipStands()
        {
            foreach (var shipStand in generatedShipStands)
            {
                GameObject.Destroy(shipStand);
            }
        }
    }

}

