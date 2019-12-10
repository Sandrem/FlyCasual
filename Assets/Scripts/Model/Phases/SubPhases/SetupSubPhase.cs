﻿using System.Collections;
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

    public class SetupSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ShipPlacement, GameCommandTypes.PressNext }; } }

        public Func<bool> SetupFilter;
        public Action SetupRangeHelper;

        private static bool inReposition;

        public Transform StartingZone { get; protected set; }
        private bool isInsideStartingZone;

        private TouchObjectPlacementHandler touchObjectPlacementHandler = new TouchObjectPlacementHandler();

        public override void Start()
        {
            base.Start();

            Name = "Setup SubPhase";
            inReposition = false;
        }

        public override void Prepare()
        {
            RequiredInitiative = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Phases.FinishSubPhase(typeof(SetupSubPhase));
        }

        public override void Next()
        {
            bool success = GetNextActivation(RequiredInitiative);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredInitiative);
                if (nextPilotSkill != int.MinValue)
                {
                    success = GetNextActivation(nextPilotSkill);
                }
                else
                {
                    FinishPhase();
                }
            }

            if (success)
            {
                UpdateHelpInfo();
                Roster.HighlightShipsFiltered(FilterShipsToSetup);

                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).SetupShip();
            }
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.State.Initiative == pilotSkill
                where n.Value.IsSetupPerformed == false
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredInitiative = pilotSkill;

                var playerNoResults =
                    from n in pilotSkillResults
                    where n.Value.Owner.PlayerNo == Phases.PlayerWithInitiative
                    select n;

                if (playerNoResults.Count() > 0)
                {
                    RequiredPlayer = Phases.PlayerWithInitiative;
                }
                else
                {
                    RequiredPlayer = Roster.AnotherPlayer(Phases.PlayerWithInitiative);
                }

                result = true;
            }

            return result;
        }

        private int GetNextPilotSkill(int pilotSkillMin)
        {
            int result = int.MinValue;

            var ascPilotSkills =
                from n in Roster.AllShips
                where !n.Value.IsSetupPerformed && n.Value.State.Initiative > pilotSkillMin
                orderby n.Value.State.Initiative
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.First().Value.State.Initiative;
            }

            return result;
        }

        public override void FinishPhase()
        {
            Board.TurnOffStartingZones();
            Board.ToggleDiceHolders(true);
            Board.ToggleOffTheBoardHolder(true);

            Phases.Events.CallSetupEnd(Phases.NextPhase);
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.State.Initiative == RequiredInitiative) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (ship.IsSetupPerformed == false)
                {
                    result = true;
                }
                else
                {
                    Messages.ShowErrorToHuman(ship.PilotInfo.PilotName + " cannot be selected: its starting position is already set");
                }
            }
            else
            {
                Messages.ShowErrorToHuman("This ship cannot be selected: the ship must be owned by " + Phases.CurrentSubPhase.RequiredPlayer + " and have a pilot skill of " + Phases.CurrentSubPhase.RequiredInitiative);
            }
            return result;
        }

        private bool FilterShipsToSetup(GenericShip ship)
        {
            return ship.State.Initiative == RequiredInitiative && !ship.IsSetupPerformed && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public static GameCommand GeneratePlaceShipCommand(int shipId, Vector3 position, Vector3 angles)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            parameters.AddField("positionX", position.x.ToString()); parameters.AddField("positionY", position.y.ToString()); parameters.AddField("positionZ", position.z.ToString());
            parameters.AddField("rotationX", angles.x.ToString()); parameters.AddField("rotationY", angles.y.ToString()); parameters.AddField("rotationZ", angles.z.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.ShipPlacement,
                typeof(SetupSubPhase),
                parameters.ToString()
            );
        }

        public static void PlaceShip(int shipId, Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            Roster.SetRaycastTargets(true);
            inReposition = false;

            Selection.ChangeActiveShip("ShipId:" + shipId);
            Selection.ThisShip.CallOnSetupPlaced();
            Board.PlaceShip(Selection.ThisShip, position, angles, delegate { Selection.DeselectThisShip(); Phases.Next(); });
        }

        public override void Update()
        {
            if (inReposition)  {
                if (CameraScript.InputMouseIsEnabled) PerformDrag();
                if (CameraScript.InputTouchIsEnabled) PerformTouchDragRotate();
            }
            CheckPerformRotation();
        }

        private void CheckPerformRotation()
        {
            if (Console.IsActive || Selection.ThisShip == null || Selection.ThisShip.Owner is Players.GenericAiPlayer) return;

            CheckResetRotation();
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RotateBy1();
            }
            else
            {
                RotateBy22_5();
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

        private void RotateBy22_5()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, -22.5f, 0));
                Selection.ThisShip.ApplyRotationHelpers();
                Selection.ThisShip.ResetRotationHelpers();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, 22.5f, 0));
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
            Selection.ThisShip.CallOnSetupSelected();
            StartDrag();
        }

        public void StartDrag()
        {
            StartingZone = Board.GetStartingZone(Phases.CurrentSubPhase.RequiredPlayer);
            isInsideStartingZone = false;
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            Board.HighlightStartingZones(Phases.CurrentSubPhase.RequiredPlayer);
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
                if (Selection.ThisShip != null)
                {
                    Selection.ThisShip.SetCenter(new Vector3(hit.point.x, 0f, hit.point.z));
                }
                else
                {
                    Debug.Log("Warning: No ship is select to drag");
                }
                
            }

            // TODO: Rework
            //CheckControlledModeLimits();

            ApplySetupPositionLimits();
            SetupRangeHelper?.Invoke();
        }

        private void PerformTouchDragRotate() {
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

        private Dictionary<string, float> GetSpaceBetween(GenericShip thisShip, GenericShip anotherShip)
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
                if ((Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player2 && (newBounds["maxZ"] < StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z))
                    || (Selection.ThisShip.Owner.PlayerNo == Players.PlayerNo.Player1 && (newBounds["minZ"] > StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z)))
                {
                    isInsideStartingZone = true;
                }
            }

            if (isInsideStartingZone)
            {
                if (SetupFilter == null)
                {
                    if (newBounds["maxZ"] > StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z) newPosition.z = StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).z - (newBounds["maxZ"] - newPosition.z + 0.01f);
                    if (newBounds["minZ"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z) newPosition.z = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).z + (newPosition.z - newBounds["minZ"] + 0.01f);
                }
                else
                {
                    if (newBounds["maxZ"] > Board.GetBoard().TransformPoint(45.72f, 45.72f, 45.72f).z) newPosition.z = Board.GetBoard().TransformPoint(45.72f, 45.72f, 45.72f).z - (newBounds["maxZ"] - newPosition.z + 0.01f);
                    if (newBounds["minZ"] < Board.GetBoard().TransformPoint(-45.72f, -45.72f, -45.72f).z) newPosition.z = Board.GetBoard().TransformPoint(-45.72f, -45.72f, -45.72f).z + (newPosition.z - newBounds["minZ"] + 0.01f);
                }
            }

            if (newBounds["maxX"] > StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).x) newPosition.x = StartingZone.TransformPoint(0.5f, 0.5f, 0.5f).x - (newBounds["maxX"] - newPosition.x + 0.01f);
            if (newBounds["minX"] < StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x) newPosition.x = StartingZone.TransformPoint(-0.5f, -0.5f, -0.5f).x + (newPosition.x - newBounds["minX"] + 0.01f);

            Selection.ThisShip.SetCenter(newPosition);
        }

        public override void ProcessClick()
        {
            if (inReposition && CameraScript.InputMouseIsEnabled)
            {
                UI.CallClickNextPhase();
            }
        }

        public bool TryConfirmPosition(GenericShip ship)
        {
            bool result = true;

            if (Phases.CurrentSubPhase.GetType() == typeof(SetupSubPhase))
            {
                if (Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().OverlapedShips.Count > 0)
                {
                    Messages.ShowErrorToHuman("This ship shouldn't overlap other ships");
                    result = false;
                }

                if (SetupFilter == null && !IsShipInStartingZone(ship))
                {
                    if (CameraScript.InputTouchIsEnabled)
                    {
                        // Touch-tailored error message
                        Messages.ShowErrorToHuman("Drag the ship into the highlighted area");
                    }
                    else
                    {
                        Messages.ShowErrorToHuman("Place the ship in the highlighted area");
                    }
                    result = false;
                }
                else if (SetupFilter != null && (!SetupFilter() && !ship.ShipBase.IsInside(StartingZone)))
                {
                    Messages.ShowErrorToHuman("This ship's position is not valid");
                    result = false;
                }
            }

            if (result) StopDrag();

            return result;
        }

        public static bool IsShipInStartingZone(GenericShip ship)
        {
            SetupSubPhase setupSubPhase = Phases.CurrentSubPhase as SetupSubPhase;

            return ship.ShipBase.IsInside(setupSubPhase.StartingZone);
        }

        public override void NextButton() {
            // Next button is only used for touch controls -- on next, try to confirm ship's position
            if (Selection.ThisShip != null && !TryConfirmPosition(Selection.ThisShip))
            {
                Console.Write("ship:" + Selection.ThisShip);
                Console.Write("shipbase:" + Selection.ThisShip.ShipBase);
                // Wait for confirmation again if positioning failed
                UI.ShowNextButton();
            }
        }

        private void StopDrag()
        {
            HideSetupHelpers();
            MovementTemplates.ReturnRangeRulers();

            Roster.SetRaycastTargets(true);
            Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().checkCollisions = false;
            inReposition = false;

            GameCommand command = GeneratePlaceShipCommand(Selection.ThisShip.ShipId, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetAngles());
            GameMode.CurrentGameMode.ExecuteCommand(command);
        }

    }

}
