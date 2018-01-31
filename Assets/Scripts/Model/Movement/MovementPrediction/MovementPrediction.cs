﻿using System.Collections;
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

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<Ship.GenericShip> ShipsBumped = new List<Ship.GenericShip>();
        public List<Collider> AsteroidsHit = new List<Collider>();
        public List<GameObject> MinesHit = new List<GameObject>();
        public bool IsLandedOnAsteroid { get; private set; }
        public float SuccessfullMovementProgress { get; private set; }
        public bool IsOffTheBoard;

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
                        IsLandedOnAsteroid = obstacleStayDetector.OverlapsAsteroid;
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
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Wait(2, delegate { DestroyGeneratedShipStands(); CallBack(); });
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

