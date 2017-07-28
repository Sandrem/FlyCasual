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
        public bool IsBumped { get; private set; }
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
            if (generatedShipStands[generatedShipStands.Length-1].GetComponentInChildren<ObstaclesStayDetector>().OverlapsShip)
            {
                IsBumped = true;
            }

            bool finalPositionFound = false;
            SuccessfullMovementProgress = 0;
            for (int i = generatedShipStands.Length-1; i >= 0; i--)
            {
                ObstaclesStayDetector obstacleStayDetector = generatedShipStands[i].GetComponentInChildren<ObstaclesStayDetector>();

                foreach (var overlapedShip in obstacleStayDetector.OverlapedShips)
                {
                    if (!Selection.ThisShip.ShipsBumped.Contains(overlapedShip))
                    {
                        Selection.ThisShip.ShipsBumped.Add(overlapedShip);
                        if (!overlapedShip.ShipsBumped.Contains(Selection.ThisShip))
                        {
                            overlapedShip.ShipsBumped.Add(Selection.ThisShip);
                        }
                    }
                }

                if ((!finalPositionFound) && (obstacleStayDetector.OverlapsShip != true))
                {
                    IsLandedOnAsteroid = obstacleStayDetector.OverlapsAsteroid;
                    SuccessfullMovementProgress = (i + 1f)/generatedShipStands.Length;
                    finalPositionFound = true;
                    break;
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

