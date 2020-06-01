using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using BoardTools;
using GameModes;
using GameCommands;
using System;
using Obstacles;
using Players;
using System.Globalization;

namespace SubPhases
{

    public class MoveObstacleMidgameSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.MoveObstacle, GameCommandTypes.PressNext }; } }

        private static bool inReposition;

        public static GenericObstacle ChosenObstacle;

        private float MinBoardEdgeDistance;
        private float MinObstaclesDistance;

        private static bool IsEnteredPlaymat = true;
        private static bool IsEnteredPlacementZone = true;
        private static bool IsPlacementBlocked;
        public static bool IsLocked;

        public Func<bool> SetupFilter;

        public static float DistanceFromEdge { get; private set; }

        private TouchObjectPlacementHandler touchObjectPlacementHandler = new TouchObjectPlacementHandler();

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            UpdateHelpInfo();

            base.Start();
        }

        public override void Prepare()
        {
            
        }

        public override void Initialize()
        {
            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            MinBoardEdgeDistance = Board.BoardIntoWorld(2 * Board.RANGE_1);
            MinObstaclesDistance = Board.BoardIntoWorld(Board.RANGE_1);

            ChosenObstacle.IsPlaced = false;

            Board.ToggleObstaclesHolder(true);
            Roster.SetRaycastTargets(false);

            ObstaclesManager.SetObstaclesCollisionDetectionQuality(CollisionDetectionQuality.Low);

            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).MoveObstacleMidgame();
        }

        public void ShowDescription()
        {
            ShowSubphaseDescription(DescriptionShort, DescriptionLong, ImageSource);
        }

        public override void Next()
        {
            FinishSubPhase();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

        public override void Update()
        {
            if (IsLocked) return;
            if (ChosenObstacle == null) return;
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

            if (CameraScript.InputTouchIsEnabled) MoveChosenObstacleTouch();
            if (CameraScript.InputMouseIsEnabled) MoveChosenObstacle();
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

        private void MoveChosenObstacleTouch()
        {
            touchObjectPlacementHandler.Update();

            if (touchObjectPlacementHandler.GetNewRotation() != 0f)
            {
                ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, touchObjectPlacementHandler.GetNewRotation(), 0);
            }

            if (touchObjectPlacementHandler.GetNewPosition() != Vector3.zero)
            {
                ChosenObstacle.ObstacleGO.transform.position = new Vector3(touchObjectPlacementHandler.GetNewPosition().x, 0f,
                                                                           touchObjectPlacementHandler.GetNewPosition().z);
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
            ApplyCustomLimits();
            ApplyObstacleLimits();
        }

        private void ApplyCustomLimits()
        {
            IsPlacementBlocked = !SetupFilter();
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
                    DistanceFromEdge = Vector3.Distance(closestPoint, hitInfo.point);
                    if (DistanceFromEdge < MinBoardEdgeDistance)
                    {
                        IsShiftRequired = true;

                        if (DistanceFromEdge < minDistance)
                        {
                            fromEdge = closestPoint;
                            toObstacle = hitInfo.point;
                            minDistance = DistanceFromEdge;
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

            foreach (MeshCollider collider in ObstaclesManager.GetPlacedObstacles().Select(n => n.ObstacleGO.GetComponentInChildren<MeshCollider>()))
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

            if (ChosenObstacle != null && CameraScript.InputMouseIsEnabled)
            {
                // For mouse input, clicking places obstacles (not for touch input though)
                TryToPlaceObstacle();
            }
        }

        private bool TryToPlaceObstacle()
        {
            if (IsEnteredPlacementZone && !IsPlacementBlocked)
            {
                GameCommand command = GenerateMoveObstacleCommand(
                    ChosenObstacle.Name,
                    ChosenObstacle.ObstacleGO.transform.position,
                    ChosenObstacle.ObstacleGO.transform.eulerAngles
                );
                GameMode.CurrentGameMode.ExecuteCommand(command);
                return true;
            }
            else
            {
                Messages.ShowError("The obstacle cannot be placed");
                return false;
            }
        }

        private GameCommand GenerateMoveObstacleCommand(string obstacleName, Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", obstacleName);
            parameters.AddField("positionX", position.x.ToString(CultureInfo.InvariantCulture));
            parameters.AddField("positionY", "0");
            parameters.AddField("positionZ", position.z.ToString(CultureInfo.InvariantCulture));

            parameters.AddField("rotationX", angles.x.ToString(CultureInfo.InvariantCulture));
            parameters.AddField("rotationY", angles.y.ToString(CultureInfo.InvariantCulture));
            parameters.AddField("rotationZ", angles.z.ToString(CultureInfo.InvariantCulture));
            return GameController.GenerateGameCommand(
                GameCommandTypes.MoveObstacle,
                typeof(MoveObstacleMidgameSubPhase),
                parameters.ToString()
            );
        }

        public static void PlaceObstacle(string obstacleName, Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            ChosenObstacle.ObstacleGO.transform.position = position;
            ChosenObstacle.ObstacleGO.transform.eulerAngles = angles;

            ChosenObstacle.ObstacleGO.transform.parent = Board.BoardTransform;

            ChosenObstacle.IsPlaced = true;
            ChosenObstacle = null;
            IsEnteredPlacementZone = false;
            IsEnteredPlaymat = false;

            MovementTemplates.ReturnRangeRulerR1();
            MovementTemplates.ReturnRangeRulerR2();

            FinishSubPhase();
        }

        private static void FinishSubPhase()
        {
            HideSubphaseDescription();

            Board.ToggleObstaclesHolder(false);
            Roster.SetRaycastTargets(true);
            ObstaclesManager.SetObstaclesCollisionDetectionQuality(CollisionDetectionQuality.High);

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            //Phases.CurrentSubPhase.Resume();
            callback();
        }

    }

}
