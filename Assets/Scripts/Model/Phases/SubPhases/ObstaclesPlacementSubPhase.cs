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

        bool touchDownLastUpdate = false; // TODO: cleaner?
        private bool mouseOverShipLastUpdate = false;
        private Vector2 lastRotationVector = Vector2.zero;


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
            Board.ToggleObstaclesHolder(true);

            MinBoardEdgeDistance = Board.BoardIntoWorld(2 * Board.RANGE_1);
            MinObstaclesDistance = Board.BoardIntoWorld(Board.RANGE_1);

            RequiredPlayer = Roster.AnotherPlayer(Phases.PlayerWithInitiative); // Will be changed in Next

            foreach (GenericPlayer player in Roster.Players)
            {
                if (player is HotacAiPlayer) IsRandomSetupSelected[player.PlayerNo] = true;
            }

            Next();
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
            CameraScript.TouchInputsPaused = false; // TODO: can move this to inside movechosenobstacle now, should still get called

            if (IsLocked) return;
            if (ChosenObstacle == null) return;
            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(HumanPlayer)) return;

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
            Vector3 pointerPosition = Vector3.zero;

            //TODO:** Use a new class for common code here and in setupsubphase I just created for touch? probably! good for encapsulation and reuse...
                // Then hopefully code in this func can be fairly minimally touched...
            // make a new touch method in this class, called by update, calls these funcs on new class:
            // .Update() -- this does the raycast and everything?
            // .GetNewPosition (vector.zero or new position)
            // .getNewRotation (rotation or 0f);
            // then that new method just appy the position / rotation it returns as needed!
            // init it when obstacle is selected:
            // .setObstacle() / setSHip()
            // private method:getObjectLocation, returns location of obstacle/ship. rest of code should be same for both...???
            if (CameraScript.InputTouchIsEnabled)
            {
                if (Input.touchCount >= 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    pointerPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                }
                else
                {
                    touchDownLastUpdate = false;
                    mouseOverShipLastUpdate = false;
                    return;
                }
            }
            else if (!CameraScript.InputTouchIsEnabled)
            {
                pointerPosition = Input.mousePosition;
            }

            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            if (Physics.Raycast(ray, out hit))
            {
              
                // On mobile, ships must be dragged instead of always moving with the mouse
                if (CameraScript.InputTouchIsEnabled)
                {
                    float distanceThreshold = .8f;
                    //TODO: pass the object to be selected to new class, have it pick which of these to use?
                    //if (Selection.ThisShip.ShipBaseSize == BaseSize.Medium) distanceThreshold = .6f;
                    //if (Selection.ThisShip.ShipBaseSize == BaseSize.Large) distanceThreshold = .8f;
                    // TODO: tweak thresholds?
                    // TODO: any way to get the actual base sizes or anything? maybe this is good enough though
                    // TODO: do the threshold in physical space not world space though? hmm might not matter

                    float distanceFromShip = (ChosenObstacle.ObstacleGO.transform.position - new Vector3(hit.point.x, 0f, hit.point.z)).magnitude;

                    if (Console.IsActive) Console.Write("distance from asteroid:" + distanceFromShip, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

                    if (touchDownLastUpdate && !mouseOverShipLastUpdate) //TODO: If I keep it here, swap the ship lingo for asteroid
                    {
                        // Don't move if something other than a ship drag is in progress
                        touchDownLastUpdate = true;
                        return;
                    }
                    else if (!mouseOverShipLastUpdate && (distanceFromShip > distanceThreshold))
                    {
                        // Don't move if the first touch is too far from the ship
                        touchDownLastUpdate = true;
                        mouseOverShipLastUpdate = false;
                        return;
                    }
                    else
                    {
                        // Otherwise, move the ship!
                        touchDownLastUpdate = true;
                        mouseOverShipLastUpdate = true;
                        CameraScript.TouchInputsPaused = true;
                    }

                    // Do ship rotation on touchscreens
                    // TODO: I think it would be better to show a seperate rotation handle or something actually, hmmm
                    if (Input.touchCount == 2)
                    {

                        Vector2 currentRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;

                        if (lastRotationVector != Vector2.zero)
                        {
                            float rotationAngle = Vector2.SignedAngle(lastRotationVector, currentRotationVector) * -2;
                            ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, rotationAngle, 0);
                        }
                        lastRotationVector = currentRotationVector;

                    }
                    else
                    {
                        lastRotationVector = Vector2.zero;
                    }

                }

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
            else if (!CameraScript.InputTouchIsEnabled)
            {
                TryToPlaceObstacle();
            }
        }

        private void TryToSelectObstacle()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // On touch devices, select on down instead of up event so drag can begin immediately
                if (Input.GetKeyUp(KeyCode.Mouse0) ||
                   (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                    {
                        if (hitInfo.transform.tag.StartsWith("Asteroid"))
                        {
                            GameObject obstacleGO = hitInfo.transform.parent.gameObject;
                            GenericObstacle clickedObstacle = ObstaclesManager.GetObstacleByName(obstacleGO.name);
                            
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

            if (Input.touchSupported)
            {
                // With touch controls, wait for confirmation before setting the position
                UI.ShowNextButton();
                IsReadyForCommands = true; //**TODO: need to adjust isreadyforcommands with the placement action too?
            }
        }


        public override void NextButton()
        {
            // Only used for touch controls
            if (!TryToPlaceObstacle()) {
                UI.ShowNextButton();
            }
        }

        private bool TryToPlaceObstacle()
        {
            if (IsEnteredPlacementZone && !IsPlacementBlocked)
            {
                GameCommand command = GeneratePlaceObstacleCommand(
                    ChosenObstacle.ObstacleGO.name,
                    ChosenObstacle.ObstacleGO.transform.position,
                    ChosenObstacle.ObstacleGO.transform.eulerAngles
                );
                GameMode.CurrentGameMode.ExecuteCommand(command);
                return true;
            }
            else
            {
                Messages.ShowError("Obstacle cannot be placed");
                return false;
            }
        }

        private GameCommand GeneratePlaceObstacleCommand(string obstacleName, Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", obstacleName);
            parameters.AddField("positionX", position.x); parameters.AddField("positionY", position.y); parameters.AddField("positionZ", position.z);
            parameters.AddField("rotationX", angles.x); parameters.AddField("rotationY", angles.y); parameters.AddField("rotationZ", angles.z);
            return GameController.GenerateGameCommand(
                GameCommandTypes.ObstaclePlacement,
                typeof(ObstaclesPlacementSubPhase),
                parameters.ToString()
            );
        }

        public static void PlaceObstacle(string obstacleName, Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            ChosenObstacle = ObstaclesManager.GetObstacleByName(obstacleName);
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
