using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using BoardTools;
using GameModes;
using GameCommands;
using System;

namespace SubPhases
{

    public class SetupShipMidgameSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ShipPlacement, GameCommandTypes.PressNext }; } }

        private static bool inReposition;

        private Transform StartingZone;
        private bool isInsideStartingZone;

        private TouchObjectPlacementHandler touchObjectPlacementHandler = new TouchObjectPlacementHandler();

        public GenericShip ShipToSetup;
        public Direction SetupSide;

        public Func<bool> SetupFilter;

        public string AbilityName;
        public string Description;
        public IImageHolder ImageSource;

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
            RequiredPlayer = ShipToSetup.Owner.PlayerNo;

            Board.ToggleOffTheBoardHolder(false);

            Board.SetShipPreSetup(ShipToSetup);

            Roster.HighlightShipsFiltered(FilterShipsToSetup);

            IsReadyForCommands = true;
            Roster.GetPlayer(RequiredPlayer).SetupShipMidgame();
        }

        public void ShowDescription()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageSource);
        }

        public override void Next()
        {
            FinishSubPhase();
        }

        private void FinishSubPhase()
        {
            HideSubphaseDescription();

            Board.TurnOffStartingZones();
            Board.ToggleOffTheBoardHolder(true);

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
            callback();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return FilterShipsToSetup(ship) && mouseKeyIsPressed == 1;
        }

        private bool FilterShipsToSetup(GenericShip ship)
        {
            return ship == ShipToSetup;
        }

        public static GameCommand GeneratePlaceShipCommand(int shipId, Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            parameters.AddField("positionX", position.x); parameters.AddField("positionY", position.y); parameters.AddField("positionZ", position.z);
            parameters.AddField("rotationX", angles.x); parameters.AddField("rotationY", angles.y); parameters.AddField("rotationZ", angles.z);
            return GameController.GenerateGameCommand(
                GameCommandTypes.ShipPlacement,
                typeof(SetupShipMidgameSubPhase),
                parameters.ToString()
            );
        }

        public static void PlaceShip(int shipId, Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            Roster.SetRaycastTargets(true);
            inReposition = false;

            Selection.ChangeActiveShip("ShipId:" + shipId);
            Board.PlaceShip(Selection.ThisShip, position, angles, delegate { Selection.DeselectThisShip(); Phases.Next(); });
        }

        public override void Update()
        {
            if (inReposition)
            {
                if (CameraScript.InputMouseIsEnabled) PerformDrag();
                if (CameraScript.InputTouchIsEnabled) PerformTouchDragRotate();
                CheckLimits();
            }
            CheckPerformRotation();
        }

        private void CheckLimits()
        {
            if (SetupFilter == null) return;

            MovementTemplates.ReturnRangeRuler();

            GenericShip nearestShip = null;
            float nearestDistance = int.MaxValue;

            foreach (GenericShip anotherShip in Roster.AllShips.Values)
            {
                if (anotherShip == Selection.ThisShip) continue;

                DistanceInfo distInfo = new DistanceInfo(Selection.ThisShip, anotherShip);
                if (distInfo.MinDistance.DistanceReal < nearestDistance)
                {
                    nearestDistance = distInfo.MinDistance.DistanceReal;
                    nearestShip = anotherShip;
                }
            }

            DistanceInfo distInfoFinal = new DistanceInfo(Selection.ThisShip, nearestShip);
            if (distInfoFinal.Range < 4)
            {
                MovementTemplates.ShowRangeRuler(distInfoFinal.MinDistance);
            }
        }

        private void CheckPerformRotation()
        {
            if (Console.IsActive) return;

            CheckResetRotation();
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RotateBy1();
            }
            else
            {
                RotateBy45();
            }
        }

        private void CheckResetRotation()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 facing = (Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -Selection.ThisShip.Model.transform.eulerAngles.y + facing.y, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }
        }

        private void RotateBy45()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -45, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, 45, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }
        }

        private void RotateBy1()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -1, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }

            if (Input.GetKey(KeyCode.E))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, 1, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            StartDrag();
        }

        public void StartDrag()
        {
            StartingZone = Board.GetStartingZone(SetupSide);
            isInsideStartingZone = false;
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            Board.HighlightStartingZones(SetupSide);
            Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().checkCollisions = true;
            inReposition = true;

            if (CameraScript.InputTouchIsEnabled)
            {
                // Setup touch handler
                touchObjectPlacementHandler.SetShip(Selection.ThisShip);

                // With touch controls, wait for confirmation before setting the position
                UI.ShowNextButton();
                IsReadyForCommands = true;
            }

        }

        private void PerformDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Selection.ThisShip.SetCenter(new Vector3(hit.point.x, 0f, hit.point.z));
            }

            CheckControlledModeLimits();
            ApplySetupPositionLimits();
        }

        private void PerformTouchDragRotate()
        {
            touchObjectPlacementHandler.Update();

            if (touchObjectPlacementHandler.GetNewRotation() != 0f)
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, touchObjectPlacementHandler.GetNewRotation(), 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }

            if (touchObjectPlacementHandler.GetNewPosition() != Vector3.zero)
            {
                Selection.ThisShip.SetCenter(new Vector3(touchObjectPlacementHandler.GetNewPosition().x, 0f,
                                                         touchObjectPlacementHandler.GetNewPosition().z));
            }
        }

        private void CheckControlledModeLimits()
        {
            // TODO: Rewrite

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                HideSetupHelpers();
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                HideSetupHelpers();

                foreach (var ship in Selection.ThisShip.Owner.Ships)
                {
                    if ((ship.Value.ShipId != Selection.ThisShip.ShipId) && (ship.Value.IsSetupPerformed))
                    {
                        Vector3 newPosition = Selection.ThisShip.GetCenter();
                        float halfOfShipStandSize = Board.BoardIntoWorld(Board.DISTANCE_1 / 2f);
                        float oneOfShipStandSize = Board.BoardIntoWorld(Board.DISTANCE_1);

                        Dictionary<string, float> spaceBetweenList = GetSpaceBetween(Selection.ThisShip, ship.Value);

                        if ((spaceBetweenList["Left"] <= halfOfShipStandSize) && (spaceBetweenList["Left"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Up"] && spaceBetweenList["Up"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Down"] && spaceBetweenList["Down"] <= 0)))
                        {
                            Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Left" : "Right")).gameObject.SetActive(true);
                            newPosition.x = newPosition.x - spaceBetweenList["Left"] + halfOfShipStandSize;
                        }
                        if ((spaceBetweenList["Right"] <= halfOfShipStandSize) && (spaceBetweenList["Right"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Up"] && spaceBetweenList["Up"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Down"] && spaceBetweenList["Down"] <= 0)))
                        {
                            Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Right" : "Left")).gameObject.SetActive(true);
                            newPosition.x = newPosition.x + spaceBetweenList["Right"] - halfOfShipStandSize;
                        }

                        if ((spaceBetweenList["Up"] <= halfOfShipStandSize) && (spaceBetweenList["Up"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Left"] && spaceBetweenList["Left"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Right"] && spaceBetweenList["Right"] <= 0)))
                        {
                            Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Top" : "Bottom")).gameObject.SetActive(true);
                            newPosition.z = newPosition.z + spaceBetweenList["Up"] - halfOfShipStandSize;
                        }
                        if ((spaceBetweenList["Down"] <= halfOfShipStandSize) && (spaceBetweenList["Down"] >= -oneOfShipStandSize) && ((-oneOfShipStandSize <= spaceBetweenList["Left"] && spaceBetweenList["Left"] <= 0) || (-oneOfShipStandSize <= spaceBetweenList["Right"] && spaceBetweenList["Right"] <= 0)))
                        {
                            Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers/Helper" + ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? "Bottom" : "Top")).gameObject.SetActive(true);
                            newPosition.z = newPosition.z - spaceBetweenList["Down"] + halfOfShipStandSize;
                        }

                        Selection.ThisShip.SetCenter(newPosition);
                    }
                }
            }
        }

        private void HideSetupHelpers()
        {
            foreach (Transform helper in Selection.ThisShip.Model.transform.Find("RotationHelper/RotationHelper2/ShipSetupHelpers").transform)
            {
                helper.gameObject.SetActive(false);
            }
        }

        private Dictionary<string, float> GetSpaceBetween(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
        {
            Dictionary<string, float> result = new Dictionary<string, float>();

            Dictionary<string, float> thisShipBounds = thisShip.ShipBase.GetBounds();
            Dictionary<string, float> anotherShipBounds = anotherShip.ShipBase.GetBounds();

            result.Add("Left", thisShipBounds["minX"] - anotherShipBounds["maxX"]);
            result.Add("Right", anotherShipBounds["minX"] - thisShipBounds["maxX"]);
            result.Add("Down", thisShipBounds["minZ"] - anotherShipBounds["maxZ"]);
            result.Add("Up", anotherShipBounds["minZ"] - thisShipBounds["maxZ"]);

            return result;
        }

        private void ApplySetupPositionLimits()
        {
            Vector3 newPosition = Selection.ThisShip.GetCenter();
            Dictionary<string, float> newBounds = Selection.ThisShip.ShipBase.GetBounds();

            if (!isInsideStartingZone)
            {
                if ((newBounds["maxZ"] < StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z) && (newBounds["minZ"] > StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z))
                {
                    isInsideStartingZone = true;
                }
            }

            if (isInsideStartingZone)
            {
                if (newBounds["maxZ"] > StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z) newPosition.z = StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z - (newBounds["maxZ"] - newPosition.z + 0.01f);
                if (newBounds["minZ"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z) newPosition.z = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z + (newPosition.z - newBounds["minZ"] + 0.01f);
            }

            if (newBounds["maxX"] > StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).x) newPosition.x = StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).x - (newBounds["maxX"] - newPosition.x + 0.01f);
            if (newBounds["minX"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x) newPosition.x = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x + (newPosition.x - newBounds["minX"] + 0.01f);

            Selection.ThisShip.SetCenter(newPosition);
        }

        public override void ProcessClick()
        {
            if (inReposition && CameraScript.InputMouseIsEnabled) TryConfirmPosition(Selection.ThisShip);
        }

        public bool TryConfirmPosition(GenericShip ship)
        {
            bool result = true;

            if (Phases.CurrentSubPhase.GetType() == typeof(SetupShipMidgameSubPhase))
            {
                if (!ship.ShipBase.IsInside(StartingZone))

                {
                    if (CameraScript.InputTouchIsEnabled)
                    {
                        // Touch-tailored error message
                        Messages.ShowErrorToHuman("Drag the ship into the highlighted area.");
                    }
                    else
                    {
                        Messages.ShowErrorToHuman("Place the ship in the highlighted area.");
                    }
                    result = false;
                }

                if (Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().OverlapedShips.Count > 0)
                {
                    Messages.ShowErrorToHuman("This ship overlaps another ship.  Please try placing it again.");
                    result = false;
                }

                if (SetupFilter != null && !SetupFilter()) return false;

            }

            if (result) StopDrag();

            return result;
        }

        public override void NextButton()
        {
            // Next button is only used for touch controls -- on next, try to confirm ship's position
            if (!TryConfirmPosition(Selection.ThisShip))
            {
                Console.Write("ship:" + Selection.ThisShip);
                Console.Write("shipbase:" + Selection.ThisShip.ShipBase);
                // Wait for confirmation again if positioning failed
                UI.ShowNextButton();
            }
        }

        private void StopDrag()
        {
            MovementTemplates.ReturnRangeRuler();

            HideSetupHelpers();
            Roster.SetRaycastTargets(true);

            Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().checkCollisions = false;
            inReposition = false;

            GameCommand command = GeneratePlaceShipCommand(Selection.ThisShip.ShipId, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetAngles());
            GameMode.CurrentGameMode.ExecuteCommand(command);
        }

    }

}
