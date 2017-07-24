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
                Debug.Log(i + ": " + obstacleStayDetector.OverlapsShip + " " + obstacleStayDetector.OverlapsAsteroid);

                if ((!finalPositionFound) && (obstacleStayDetector.OverlapsShip != true))
                {
                    IsLandedOnAsteroid = obstacleStayDetector.OverlapsAsteroid;
                    SuccessfullMovementProgress = (i + 1f)/generatedShipStands.Length;
                    finalPositionFound = true;
                    break;
                }
            }
            //TODO: What if no movement at all?!

            Selection.ThisShip.ToggleColliders(true);

            //Debug.Log("Bumped into ship: " + IsBumped);
            //Debug.Log("Landed on asteroid: " + IsLandedOnAsteroid);
            //Debug.Log("Successfull Movement Progress: " + SuccessfullMovementProgress);

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

