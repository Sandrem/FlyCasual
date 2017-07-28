using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class MovementPrediction
    {
        private int updatesCount = 0;
        private GenericMovement currentMovement;
        private GameObject[] generatedShipStands;

        private bool isBumped;
        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<Ship.GenericShip> ShipsBumped = new List<Ship.GenericShip>();
        public bool IsLandedOnAsteroid { get; private set; }
        public float SuccessfullMovementProgress { get; private set; }

        public MovementPrediction(GenericMovement movement)
        {
            currentMovement = movement;
            Selection.ThisShip.ToggleColliders(false);
            GenerateShipStands();
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        private void GenerateShipStands()
        {
            generatedShipStands = currentMovement.PlanMovement();
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
            for (int i = generatedShipStands.Length-1; i >= 0; i--)
            {
                ObstaclesStayDetector obstacleStayDetector = generatedShipStands[i].GetComponentInChildren<ObstaclesStayDetector>();
                ObstaclesStayDetector lastShipBumpDetector = null;

                if (!finalPositionFound)
                {
                    if (!obstacleStayDetector.OverlapsShip)
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

                        finalPositionFound = true;
                        break;
                    }
                }
                else
                {
                    lastShipBumpDetector = obstacleStayDetector;
                }
                
            }

            Selection.ThisShip.ToggleColliders(true);

            DestroyGeneratedShipStands();

            currentMovement.LaunchShipMovement();
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

