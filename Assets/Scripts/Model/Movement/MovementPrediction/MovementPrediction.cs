using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class MovementPrediction
    {
        private Action CallBack;

        private int updatesCount = 0;
        public GenericMovement CurrentMovement;
        private GameObject[] generatedShipStands;

        private bool isBumped;
        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<Ship.GenericShip> ShipsBumped = new List<Ship.GenericShip>();
        public List<Collider> AsteroidsHit = new List<Collider>();
        public bool IsLandedOnAsteroid { get; private set; }
        public float SuccessfullMovementProgress { get; private set; }

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

            for (int i = generatedShipStands.Length-1; i >= 0; i--)
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
                        IsLandedOnAsteroid = obstacleStayDetector.OverlapsAsteroid;
                        SuccessfullMovementProgress = (i + 1f) / generatedShipStands.Length;

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

                        finalPositionFound = true;
                        //break;
                    }
                }
                else
                {
                    foreach (var asteroidHit in obstacleHitsDetector.OverlapedAsteroids)
                    {
                        if (!AsteroidsHit.Contains(asteroidHit))
                        {
                            AsteroidsHit.Add(asteroidHit);
                        }
                    }
                }

            }

            Selection.ThisShip.ToggleColliders(true);

            DestroyGeneratedShipStands();

            CallBack();
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

