using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BoardTools;
using UnityEngine.EventSystems;
using Obstacles;
using Players;
using UnityEngine.UI;
using System;

namespace SubPhases
{

    public class ObstaclesPlacementSubPhase : GenericSubPhase
    {
        public GenericObstacle ChosenObstacle;
        private float MinBoardEdgeDistance;
        private float MinObstaclesDistance;
        private bool IsPlacementBlocked;
        private bool IsEnteredPlaymat;
        private bool IsEnteredPlacementZone;

        public override void Start()
        {
            Name = "Obstacles Setup";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Board.ToggleObstaclesHolder(true);

            MinBoardEdgeDistance = Board.BoardIntoWorld(2 * Board.RANGE_1);
            MinObstaclesDistance = Board.BoardIntoWorld(Board.RANGE_1);

            RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);
            Next();
        }

        public override void Next()
        {
            HideSubphaseDescription();

            RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);
            ShowSubphaseDescription(Name, "Obstacles cannot be placed at Range 1 of each other, or at Range 1-2 of an edge of the play area.");
            if (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(HumanPlayer)) UI.ShowSkipButton("Random");
            Roster.HighlightPlayer(RequiredPlayer);

            Roster.GetPlayer(RequiredPlayer).PlaceObstacle();
        }

        private void FinishSubPhase()
        {
            HideSubphaseDescription();

            Board.ToggleObstaclesHolder(false);

            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartSetupPhase);
            (subphase as NotificationSubPhase).TextToShow = "Setup";
            subphase.Start();
        }

        private void StartSetupPhase()
        {
            Phases.CurrentSubPhase = new SetupStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

        public override void Update()
        {
            if (ChosenObstacle == null) return;

            MoveChosenObstacle();
            CheckPerformRotation();

            CheckEntered();
            if (IsEnteredPlaymat) CheckLimits();
        }

        private void CheckEntered()
        {
            Vector3 position = ChosenObstacle.ObstacleGO.transform.position;

            if (!IsEnteredPlacementZone)
            {
                if (Mathf.Abs(position.x) < 2.7f && Mathf.Abs(position.z) < 2.7f)
                {
                    IsEnteredPlacementZone = true;
                }
            }

            if (!IsEnteredPlaymat)
            {
                if (Mathf.Abs(position.x) < 5f && Mathf.Abs(position.z) < 5f)
                {
                    IsEnteredPlaymat = true;
                }
            }

            if (IsEnteredPlaymat)
            {
                if (position.x < -4.5f) ChosenObstacle.ObstacleGO.transform.position = new Vector3(-4.5f, position.y, position.z);
                if (position.x > 4.5f) ChosenObstacle.ObstacleGO.transform.position = new Vector3(4.5f, position.y, position.z);

                position = ChosenObstacle.ObstacleGO.transform.position;
                if (position.z < -4.5f) ChosenObstacle.ObstacleGO.transform.position = new Vector3(position.x, position.y, -4.5f);
                if (position.z > 4.5f) ChosenObstacle.ObstacleGO.transform.position = new Vector3(position.x, position.y, 4.5f);
            }
        }

        private void MoveChosenObstacle()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                ChosenObstacle.ObstacleGO.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
            }
        }

        private void CheckPerformRotation()
        {
            if (Console.IsActive) return;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                RotateBy1();
            }
            else
            {
                RotateBy45();
            }
        }

        private void RotateBy45()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, -45, 0);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, 45, 0);
            }
        }

        private void RotateBy1()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, -1, 0);
            }

            if (Input.GetKey(KeyCode.E))
            {
                ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, 1, 0);
            }
        }

        private void CheckLimits()
        {
            IsPlacementBlocked = false;
            ApplyEdgeLimits();
            ApplyObstacleLimits();
        }

        private void ApplyEdgeLimits()
        {
            MovementTemplates.ReturnRangeRulerR2();

            Vector3 fromEdge = Vector3.zero;
            Vector3 toObstacle = Vector3.zero;
            float minDistance = float.MaxValue;

            bool IsShiftRequired = false;

            foreach (BoxCollider collider in Board.BoardTransform.Find("OffTheBoardHolder").GetComponentsInChildren<BoxCollider>())
            {
                Vector3 closestPoint = collider.ClosestPoint(ChosenObstacle.ObstacleGO.transform.position);

                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Raycast(closestPoint + new Vector3(0, 0.003f, 0), ChosenObstacle.ObstacleGO.transform.position - closestPoint, out hitInfo))
                {
                    float distanceFromEdge = Vector3.Distance(closestPoint, hitInfo.point);
                    if (distanceFromEdge < MinBoardEdgeDistance)
                    {
                        IsShiftRequired = true;

                        if (distanceFromEdge < minDistance)
                        {
                            fromEdge = closestPoint;
                            toObstacle = hitInfo.point;
                            minDistance = distanceFromEdge;
                        }

                        MoveObstacleToKeepInPlacementZone(closestPoint, hitInfo.point);
                    }
                }
            }

            if (IsShiftRequired)
            {
                MovementTemplates.ShowRangeRulerR2(fromEdge, toObstacle);
            }
        }

        private void MoveObstacleToKeepInPlacementZone(Vector3 pointOnEdge, Vector3 nearestPoint)
        {
            Vector3 disallowedVector = nearestPoint - pointOnEdge;
            Vector3 allowedVector = disallowedVector / Vector3.Distance(pointOnEdge, nearestPoint) * MinBoardEdgeDistance;
            
            Vector3 shift = (allowedVector - disallowedVector);
            ChosenObstacle.ObstacleGO.transform.position += shift;
        }

        private void ApplyObstacleLimits()
        {
            MovementTemplates.ReturnRangeRulerR1();

            Vector3 fromObstacle = Vector3.zero;
            Vector3 toObstacle = Vector3.zero;
            float minDistance = float.MaxValue;

            bool isBlockedByAnotherObstacle = false;

            foreach (MeshCollider collider in ObstaclesManager.Instance.GetPlacedObstacles().Select(n => n.ObstacleGO.GetComponentInChildren<MeshCollider>()))
            {
                Vector3 closestPoint = collider.ClosestPoint(ChosenObstacle.ObstacleGO.transform.position);

                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Raycast(closestPoint + new Vector3(0, 0.003f, 0), ChosenObstacle.ObstacleGO.transform.position - closestPoint, out hitInfo))
                {
                    float distanceBetween = Vector3.Distance(closestPoint, hitInfo.point);
                    if (distanceBetween < MinObstaclesDistance)
                    {
                        IsPlacementBlocked = true;
                        isBlockedByAnotherObstacle = true;

                        if (distanceBetween < minDistance)
                        {
                            fromObstacle = closestPoint + new Vector3(0, 0.003f, 0);
                            toObstacle = hitInfo.point;
                            minDistance = distanceBetween;
                        }
                    }
                }

                // In case if one asteroid is inside of another
                float distanceToCenter = Vector3.Distance(closestPoint, ChosenObstacle.ObstacleGO.transform.position);
                if (distanceToCenter < MinObstaclesDistance)
                {
                    IsPlacementBlocked = true;
                    isBlockedByAnotherObstacle = true;

                    if (distanceToCenter < minDistance)
                    {
                        fromObstacle = closestPoint;
                        toObstacle = ChosenObstacle.ObstacleGO.transform.position;
                        minDistance = distanceToCenter;
                    }
                }
            }

            if (IsPlacementBlocked && isBlockedByAnotherObstacle)
            {
                MovementTemplates.ShowRangeRulerR1(fromObstacle, toObstacle);
            }
        }

        public override void ProcessClick()
        {
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

            if (ChosenObstacle == null)
            {
                TryToSelectObstacle();
            }
            else
            {
                TryToPlaceObstacle();
            }
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Asteroid"))
                        {
                            GameObject obstacleGO = hitInfo.transform.parent.gameObject;
                            GenericObstacle clickedObstacle = ObstaclesManager.Instance.GetObstacleByName(obstacleGO.name);
                            
                            if (!clickedObstacle.IsPlaced)
                            {
                                SelectObstacle(clickedObstacle);
                            }
                        }
                    }
                }
            }
        }

        private void SelectObstacle(GenericObstacle obstacle)
        {
            Board.ToggleOffTheBoardHolder(true);
            ChosenObstacle = obstacle;
            UI.HideSkipButton();
        }

        private void TryToPlaceObstacle()
        {
            if (IsEnteredPlacementZone && !IsPlacementBlocked)
            {
                Board.ToggleOffTheBoardHolder(false);

                ChosenObstacle.ObstacleGO.transform.parent = Board.BoardTransform;

                ChosenObstacle.IsPlaced = true;
                ChosenObstacle = null;
                IsEnteredPlacementZone = false;
                IsEnteredPlaymat = false;

                MovementTemplates.ReturnRangeRulerR2();

                if (ObstaclesManager.Instance.GetPlacedObstaclesCount() < 6)
                {
                    Next();
                }
                else
                {
                    FinishSubPhase();
                }
            }
            else
            {
                Messages.ShowError("Obstacle cannot be placed");
            }
        }

        public override void SkipButton()
        {
            PlaceRandom();
        }

        public void PlaceRandom()
        {
            GetRandomObstacle();
            SetRandomPosition();
        }

        private void GetRandomObstacle()
        {
            SelectObstacle(ObstaclesManager.Instance.GetRandomFreeObstacle());
        }

        private void SetRandomPosition()
        {
            float randomX = UnityEngine.Random.Range(-2.7f, 2.7f);
            float randomZ = UnityEngine.Random.Range(-2.7f, 2.7f);

            ChosenObstacle.ObstacleGO.transform.position = new Vector3(randomX, 0, randomZ);
            CheckEntered();
            CheckLimits();
            if (!IsPlacementBlocked)
            {
                TryToPlaceObstacle();
            }
            else
            {
                SetRandomPosition();
            }
        }
    }

}
