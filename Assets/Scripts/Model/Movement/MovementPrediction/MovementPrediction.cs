using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Obstacles;
using Ship;
using Bombs;
using Remote;
using System.Linq;

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

        public List<GenericShip> ShipsBumped = new List<GenericShip>();
        public List<GenericRemote> RemotesOverlapped = new List<GenericRemote>();
        public List<GenericRemote> RemotesMovedThrough = new List<GenericRemote>();
        public List<GenericObstacle> AsteroidsHit = new List<GenericObstacle>();
        public List<GenericDeviceGameObject> MinesHit = new List<GenericDeviceGameObject>();
        public bool IsLandedOnAsteroid { get { return LandedOnObstacles.Count > 0; } }
        public List<GenericObstacle> LandedOnObstacles = new List<GenericObstacle>();
        public float SuccessfullMovementProgress { get; private set; }
        public bool IsOffTheBoard;

        public ShipPositionInfo FinalPositionInfo { get; private set; }
        public Vector3 FinalPosition { get; private set; }
        public Vector3 FinalAngles { get; private set; }

        public MovementPrediction(GenericMovement movement)
        {
            CurrentMovement = movement;

            Selection.ThisShip.ToggleColliders(false);
        }

        public IEnumerator CalculateMovementPredicition()
        {
            yield return UpdateColisionDetectionAlt();
        }

        public void CalculateOnlyFinalPosition()
        {
            if (CurrentMovement.RotationEndDegrees != 0)
            {
                Vector3 centerOfTempBase = generatedShipStands[generatedShipStands.Length - 1].transform.TransformPoint(new Vector3(0, 0, -Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE));
                generatedShipStands[generatedShipStands.Length - 1].transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), CurrentMovement.RotationEndDegrees);
            }

            FinalPosition = generatedShipStands[generatedShipStands.Length - 1].transform.position;
            FinalAngles = generatedShipStands[generatedShipStands.Length - 1].transform.eulerAngles;
            FinalPositionInfo = new ShipPositionInfo(FinalPosition, FinalAngles);

            if (!DebugManager.DebugMovementDestroyTempBasesLater)
            {
                DestroyGeneratedShipStands();
            }
            else
            {
                GameManagerScript.Wait(2, DestroyGeneratedShipStands);
            }
        }

        public MovementPrediction(GenericMovement movement, Action callBack)
        {
            CurrentMovement = movement;
            CallBack = callBack;

            Selection.ThisShip.ToggleColliders(false);
            GenerateShipStands();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        public void GenerateShipStands()
        {
            generatedShipStands = CurrentMovement.PlanMovement();

            FinalPosition = generatedShipStands.Last().transform.position;
            FinalAngles = generatedShipStands.Last().transform.eulerAngles;
            FinalPositionInfo = new ShipPositionInfo(FinalPosition, FinalAngles);
            CurrentMovement.FinalPositionInfo = FinalPositionInfo;

            if (CurrentMovement.RotationEndDegrees != 0)
            {
                Vector3 centerOfTempBase = generatedShipStands.Last().transform.TransformPoint(new Vector3(0, 0, -Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE));
                generatedShipStands.Last().transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), CurrentMovement.RotationEndDegrees);
            }
        }

        public void GenerateFinalShipStand()
        {
            generatedShipStands = CurrentMovement.PlanFinalPosition();
        }

        private IEnumerator UpdateColisionDetectionAlt()
        {
            yield return WaitForFrames(2);
            GetResults();
        }

        public static IEnumerator WaitForFrames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
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

                        foreach (var overlapedRemote in obstacleStayDetector.OverlapedRemotes)
                        {
                            if (!RemotesOverlapped.Contains(overlapedRemote))
                            {
                                RemotesOverlapped.Add(overlapedRemote);
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
                            GenericDeviceGameObject MineObject = mineHit.transform.parent.GetComponent<GenericDeviceGameObject>();
                            if (!MinesHit.Contains(MineObject))
                            {
                                MinesHit.Add(MineObject);
                            }
                        }

                        finalPositionFound = true;

                        if (i != generatedShipStands.Length - 1)
                        {
                            FinalPosition = generatedShipStands[i].transform.position;
                            FinalAngles = generatedShipStands[i].transform.eulerAngles;
                            FinalPositionInfo = new ShipPositionInfo(FinalPosition, FinalAngles);
                            CurrentMovement.FinalPositionInfo = FinalPositionInfo;
                        }

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
                        GenericDeviceGameObject MineObject = mineHit.transform.parent.GetComponent<GenericDeviceGameObject>();
                        if (!MinesHit.Contains(MineObject))
                        {
                            MinesHit.Add(MineObject);
                        }
                    }

                    foreach (var remoteMovedThrough in obstacleHitsDetector.RemotesMovedThrough)
                    {
                        if (!RemotesMovedThrough.Contains(remoteMovedThrough))
                        {
                            RemotesMovedThrough.Add(remoteMovedThrough);
                        }
                    }
                }

            }

            Selection.ThisShip.ToggleColliders(true);

            if (!DebugManager.DebugMovementDestroyTempBasesLater)
            {
                DestroyGeneratedShipStands();
                if (CallBack != null) CallBack();
            }
            else
            {
                GameManagerScript.Wait(2, delegate { DestroyGeneratedShipStands(); if (CallBack != null) CallBack(); });
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

