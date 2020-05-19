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
        public GenericShip Ship;
        public GenericMovement CurrentMovement;
        public ShipPositionInfo FinalPositionInfo { get; private set; }
        public ShipPositionInfo FinalPositionBeforeRotationInfo { get; private set; }
        public float SuccessfullMovementProgress { get; private set; }
        public bool IsOffTheBoard;
        public bool IsBumped { get { return ShipsBumped.Count != 0; } }
        public List<GenericShip> ShipsBumped = new List<GenericShip>();
        public List<GenericRemote> RemotesOverlapped = new List<GenericRemote>();
        public List<GenericRemote> RemotesMovedThrough = new List<GenericRemote>();
        public List<GenericObstacle> AsteroidsHit = new List<GenericObstacle>();
        public List<GenericDeviceGameObject> MinesHit = new List<GenericDeviceGameObject>();
        public bool IsLandedOnAsteroid { get { return LandedOnObstacles.Count > 0; } }
        public List<GenericObstacle> LandedOnObstacles = new List<GenericObstacle>();

        private GameObject[] GeneratedShipStands;

        public MovementPrediction(GenericShip ship, GenericMovement movement)
        {
            Ship = ship;
            CurrentMovement = movement;
        }

        public IEnumerator CalculateMovementPredicition()
        {
            DisableCollisionDetectionAtCurrentPosition();
            GenerateShipStands();
            yield return UpdateColisionDetection();
            EnableCollisionDetectionAtCurrentPosition();
            PerformCleanup();
        }

        private void DisableCollisionDetectionAtCurrentPosition()
        {
            Ship.ToggleColliders(false);
        }

        private void EnableCollisionDetectionAtCurrentPosition()
        {
            Ship.ToggleColliders(true);
        }

        private void GenerateShipStands()
        {
            GeneratedShipStands = CurrentMovement.PlanMovement();

            if (CurrentMovement.HasRotationInTheEnd)
            {
                RotateAroundCenter(GeneratedShipStands.Last(), CurrentMovement.RotationEndDegrees);
            }
        }

        private void RotateAroundCenter(GameObject shipStand, int degrees)
        {
            Vector3 centerOfTempBase = shipStand.transform.TransformPoint(new Vector3(0, 0, -Ship.ShipBase.HALF_OF_SHIPSTAND_SIZE));
            shipStand.transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), degrees);
        }

        private IEnumerator UpdateColisionDetection()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            GetResults();
        }

        private void GetResults()
        {
            bool finalPositionFound = false;
            ObstaclesStayDetector lastShipBumpDetector = null;

            for (int i = GeneratedShipStands.Length - 1; i >= 0; i--)
            {
                ObstaclesStayDetector obstacleStayDetector = GeneratedShipStands[i].GetComponentInChildren<ObstaclesStayDetector>();
                ObstaclesHitsDetector obstacleHitsDetector = GeneratedShipStands[i].GetComponentInChildren<ObstaclesHitsDetector>();

                if (!finalPositionFound)
                {
                    if (obstacleStayDetector.OverlapsShip)
                    {
                        // Save information in which ships we are bumped
                        lastShipBumpDetector = obstacleStayDetector;
                    }
                    else
                    {
                        finalPositionFound = true;
                        SuccessfullMovementProgress = (float)(i) / (GeneratedShipStands.Length - 1);

                        ProcessBumpedShips(lastShipBumpDetector);

                        ProcessFinalPosition(i);
                        ProcessOffTheBoard(obstacleStayDetector);
                        ProcessObstaclesLanded(obstacleStayDetector);
                        ProcessRemotesOverlaps(obstacleStayDetector);
                        ProcessObstaclesHit(obstacleStayDetector);
                        ProcessMines(obstacleStayDetector);
                    }
                }
                else
                {
                    ProcessStandOnPath(obstacleHitsDetector);
                }

            }
        }

        private void ProcessFinalPosition(int index)
        {
            SaveFinalPositionInfo(GeneratedShipStands[index]);

            if (CurrentMovement.HasRotationInTheEnd && SuccessfullMovementProgress == 1)
            {
                RotateAroundCenter(GeneratedShipStands[index], -CurrentMovement.RotationEndDegrees);
                SaveFinalPositionInfoBeforeRotation(GeneratedShipStands[index]);
                RotateAroundCenter(GeneratedShipStands[index], CurrentMovement.RotationEndDegrees);
            }
            else
            {
                SaveFinalPositionInfoBeforeRotation(GeneratedShipStands[index]);
            }
        }

        private void SaveFinalPositionInfo(GameObject shipStand)
        {
            FinalPositionInfo = new ShipPositionInfo
            (
                shipStand.transform.position,
                shipStand.transform.eulerAngles
            );
            CurrentMovement.FinalPositionInfo = FinalPositionInfo;
        }

        private void SaveFinalPositionInfoBeforeRotation(GameObject shipStand)
        {
            FinalPositionBeforeRotationInfo = new ShipPositionInfo
            (
                shipStand.transform.position,
                shipStand.transform.eulerAngles
            );
            CurrentMovement.FinalPositionInfoBeforeRotation = FinalPositionBeforeRotationInfo;
        }

        private void ProcessBumpedShips(ObstaclesStayDetector lastShipBumpDetector)
        {
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
        }

        private void ProcessOffTheBoard(ObstaclesStayDetector obstacleStayDetector)
        {
            IsOffTheBoard = obstacleStayDetector.OffTheBoard;
        }

        private void ProcessObstaclesLanded(ObstaclesStayDetector obstacleStayDetector)
        {
            LandedOnObstacles = new List<GenericObstacle>(obstacleStayDetector.OverlapedAsteroids);
        }

        private void ProcessRemotesOverlaps(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (var overlapedRemote in obstacleStayDetector.OverlapedRemotes)
            {
                if (!RemotesOverlapped.Contains(overlapedRemote))
                {
                    RemotesOverlapped.Add(overlapedRemote);
                }
            }
        }

        private void ProcessObstaclesHit(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (var asteroidHit in obstacleStayDetector.OverlapedAsteroids)
            {
                if (!AsteroidsHit.Contains(asteroidHit))
                {
                    AsteroidsHit.Add(asteroidHit);
                }
            }
        }

        private void ProcessMines(ObstaclesStayDetector obstacleStayDetector)
        {
            foreach (var mineHit in obstacleStayDetector.OverlapedMines)
            {
                GenericDeviceGameObject MineObject = mineHit.transform.parent.GetComponent<GenericDeviceGameObject>();
                if (!MinesHit.Contains(MineObject))
                {
                    MinesHit.Add(MineObject);
                }
            }
        }

        private void ProcessStandOnPath(ObstaclesHitsDetector obstacleHitsDetector)
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

        private void PerformCleanup()
        {
            if (!DebugManager.DebugMovementDestroyTempBasesLater)
            {
                DestroyGeneratedShipStands();
            }
            else
            {
                GameManagerScript.Wait(2, DestroyGeneratedShipStands);
            }
        }

        private void DestroyGeneratedShipStands()
        {
            foreach (var shipStand in GeneratedShipStands)
            {
                GameObject.Destroy(shipStand);
            }
        }

        // Calculation of only final position

        public void CalculateOnlyFinalPositionIgnoringCollisions()
        {
            DisableCollisionDetectionAtCurrentPosition();
            GenerateFinalShipStand();
            // TODO: GET FINAL POSITION
            EnableCollisionDetectionAtCurrentPosition();
            PerformCleanup();
        }

        private void GenerateFinalShipStand()
        {
            GeneratedShipStands = CurrentMovement.PlanFinalPosition();

            SaveFinalPositionInfoBeforeRotation(GeneratedShipStands.Last());

            if (CurrentMovement.HasRotationInTheEnd)
            {
                Vector3 centerOfTempBase = GeneratedShipStands.Last().transform.TransformPoint(new Vector3(0, 0, -Ship.ShipBase.HALF_OF_SHIPSTAND_SIZE));
                GeneratedShipStands.Last().transform.RotateAround(centerOfTempBase, new Vector3(0, 1, 0), CurrentMovement.RotationEndDegrees);
            }

            SaveFinalPositionInfo(GeneratedShipStands.Last());
        }
    }

}

