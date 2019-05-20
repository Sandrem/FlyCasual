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
using GameModes;
using GameCommands;

namespace SubPhases
{

    public class ObstaclesPlacementSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ObstaclePlacement, GameCommandTypes.PressSkip, GameCommandTypes.PressNext }; } }

        public static GenericObstacle ChosenObstacle;
        private float MinBoardEdgeDistance;
        private float MinObstaclesDistance;
        private static bool IsPlacementBlocked;
        private static bool IsEnteredPlaymat;
        private static bool IsEnteredPlacementZone;
        public static bool IsLocked;

        private TouchObjectPlacementHandler touchObjectPlacementHandler = new TouchObjectPlacementHandler();

        public Dictionary<PlayerNo, bool> IsRandomSetupSelected = new Dictionary<PlayerNo, bool>()
        {
            { PlayerNo.Player1, false },
            { PlayerNo.Player2, false }
        };

        public override void Start()
        {
            Name = "Obstacle Setup";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            ShowObstaclesHolder();

            MinBoardEdgeDistance = Board.BoardIntoWorld(2 * Board.RANGE_1);
            MinObstaclesDistance = Board.BoardIntoWorld(Board.RANGE_1);

            RequiredPlayer = Roster.AnotherPlayer(Phases.PlayerWithInitiative); // Will be changed in Next

            foreach (GenericPlayer player in Roster.Players)
            {
                if (player.PlayerType == PlayerType.Ai) IsRandomSetupSelected[player.PlayerNo] = true;
            }

            Next();
        }

        private void ShowObstaclesHolder()
        {
            Board.ToggleObstaclesHolder(true);

            int asteroidCount = 1;

            for (int i = 1; i < 3; i++)
            {
                foreach (GenericObstacle obstacle in Roster.GetPlayer(i).ChosenObstacles)
                {
                    GameObject obstacleHolder = Board.GetObstacleHolder().Find("Obstacle" + asteroidCount++).gameObject;
                    GameObject obstacleModelPrefab = Resources.Load<GameObject>(string.Format("Prefabs/Obstacles/{0}/{1}", obstacle.GetTypeName, obstacle.Name));
                    obstacle.ObstacleGO = GameObject.Instantiate<GameObject>(obstacleModelPrefab, obstacleHolder.transform);
                    obstacle.Name = obstacle.Name + "_" + i;
                    obstacle.ObstacleGO.name = obstacle.Name;
                    GameObject colliderGO = obstacle.ObstacleGO.transform.Find("default").gameObject;
                    colliderGO.name = obstacle.Name;
                    Board.Objects.Add(colliderGO.GetComponent<MeshCollider>());
                }
            }
        }

        public override void Next()
        {
            IsLocked = true;
            HideSubphaseDescription();

            RequiredPlayer = Roster.AnotherPlayer(RequiredPlayer);
            if (!IsRandomSetupSelected[RequiredPlayer]) ShowSubphaseDescription(Name, "Obstacles cannot be placed at Range 1 of each other, or at Range 1-2 of an edge of the play area.");

            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).PlaceObstacle();
        }

        private static void FinishSubPhase()
        {
            HideSubphaseDescription();

            Board.ToggleObstaclesHolder(false);

            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartSetupPhase);
            (subphase as NotificationSubPhase).TextToShow = "Setup";
            subphase.Start();
        }

        private static void StartSetupPhase()
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

        private void MoveChosenObstacleTouch() {
            touchObjectPlacementHandler.Update();

            if (touchObjectPlacementHandler.GetNewRotation() != 0f) {
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

            if (ChosenObstacle == null)
            {
                TryToSelectObstacle();
            }
            else if (CameraScript.InputMouseIsEnabled)
            {
                // For mouse input, clicking places obstacles (not for touch input though)
                TryToPlaceObstacle();
            }
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject() &&
               (Input.touchCount == 0 || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {
                // On touch devices, select on down instead of up event so drag can begin immediately
                if (Input.GetKeyUp(KeyCode.Mouse0) ||
                    (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    bool castHit = Physics.Raycast(ray, out hitInfo);

                    // If an asteroid wasn't found and we're on touch, see if the user tapped right next to an asteroid
                    // Since the asteroid can be small, they can be hard to touch and this helps with that
                    if (CameraScript.InputTouchIsEnabled && 
                        (!castHit || !hitInfo.transform.tag.StartsWith("Obstacle"))) {
                       
                        castHit = Physics.SphereCast(ray, 0.1f, out hitInfo, 10f);
                    }


                    // Select the obstacle found if it's valid
                    if (castHit && hitInfo.transform.tag.StartsWith("Obstacle"))
                    {
                        GameObject obstacleGO = hitInfo.transform.parent.gameObject;
                        GenericObstacle clickedObstacle = ObstaclesManager.GetChosenObstacle(obstacleGO.transform.name);

                        if (!clickedObstacle.IsPlaced)
                        {
                            SelectObstacle(clickedObstacle);
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

            if (CameraScript.InputTouchIsEnabled && !IsRandomSetupSelected[RequiredPlayer])
            {
                // With touch controls, wait for confirmation before setting the position
                UI.ShowNextButton();
                IsReadyForCommands = true;

                // Set up touch handler
                touchObjectPlacementHandler.SetObstacle(obstacle);
            }
        }


        public override void NextButton()
        {
            // Only used for touch controls -- try to confirm obstacle placement
            if (!TryToPlaceObstacle()) {
                UI.ShowNextButton();
            }
        }

        private bool TryToPlaceObstacle()
        {
            if (IsEnteredPlacementZone && !IsPlacementBlocked)
            {
                GameCommand command = GeneratePlaceObstacleCommand(
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

        private GameCommand GeneratePlaceObstacleCommand(string obstacleName, Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", obstacleName);
            parameters.AddField("positionX", position.x.ToString()); parameters.AddField("positionY", position.y.ToString()); parameters.AddField("positionZ", position.z.ToString());
            parameters.AddField("rotationX", angles.x.ToString()); parameters.AddField("rotationY", angles.y.ToString()); parameters.AddField("rotationZ", angles.z.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.ObstaclePlacement,
                typeof(ObstaclesPlacementSubPhase),
                parameters.ToString()
            );
        }

        public static void PlaceObstacle(string obstacleName,  Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            ChosenObstacle = ObstaclesManager.GetChosenObstacle(obstacleName);
            ChosenObstacle.ObstacleGO.transform.position = position;
            ChosenObstacle.ObstacleGO.transform.eulerAngles = angles;

            Board.ToggleOffTheBoardHolder(false);

            ChosenObstacle.ObstacleGO.transform.parent = Board.BoardTransform;

            ChosenObstacle.IsPlaced = true;
            ChosenObstacle = null;
            IsEnteredPlacementZone = false;
            IsEnteredPlaymat = false;

            MovementTemplates.ReturnRangeRulerR1();
            MovementTemplates.ReturnRangeRulerR2();

            if (ObstaclesManager.GetPlacedObstaclesCount() < 6)
            {
                Phases.CurrentSubPhase.Next();
            }
            else
            {
                FinishSubPhase();
            }
        }

        public override void SkipButton()
        {
            IsRandomSetupSelected[RequiredPlayer] = true;
            PlaceRandom();
        }

        public void PlaceRandom()
        {
            GetRandomObstacle();
            SetRandomPosition();
        }

        private void GetRandomObstacle()
        {
            SelectObstacle(ObstaclesManager.GetRandomFreeObstacle());
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
