using System.Collections.Generic;
using UnityEngine;
using Ship;
using BoardTools;
using GameModes;
using GameCommands;
using System;
using Remote;
using Players;

namespace SubPhases
{

    public class RemoteSetupSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ShipPlacement, GameCommandTypes.PressNext }; } }

        private static bool inReposition;

        private bool IsLocked;

        private TouchObjectPlacementHandler touchObjectPlacementHandler = new TouchObjectPlacementHandler();

        public GenericPlayer RemoteOwner { get; private set; }
        public Type RemoteType { get; private set; }
        public float MinBoardEdgeDistance { get; set; }
        public float DistanceFromEdge { get; private set; }
        public bool IsPositionWrong { get; private set; }

        public void PrepareRemoteSetup(Type remoteType, GenericPlayer remoteOwner)
        {
            RemoteType = remoteType;
            RemoteOwner = remoteOwner;
            RequiredPlayer = remoteOwner.PlayerNo;
        }

        public override void Start()
        {
            base.Start();

            Name = "Remote Setup SubPhase";
            inReposition = false;

            GenericRemote remote = ShipFactory.SpawnRemote(
                (GenericRemote)Activator.CreateInstance(RemoteType, RemoteOwner),
                Vector3.zero,
                new Quaternion(0, 0, 0, 0)
            );

            Selection.ThisShip = remote;

            if (RemoteOwner is HumanPlayer)
            {
                ShowSubphaseDescription(DescriptionShort, DescriptionLong, ImageSource);
            }

            Board.ToggleOffTheBoardHolder(true);

            StartDrag();

            Roster.GetPlayer(RequiredPlayer).SetupRemote();
        }

        public override void Next()
        {
            FinishPhase();
        }

        public override void FinishPhase()
        {
            HideSubphaseDescription();
            Selection.DeselectThisShip();
            Board.ToggleOffTheBoardHolder(false);

            Phases.CurrentSubPhase = PreviousSubPhase;
            CallBack();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public static void PlaceShip(int shipId, Vector3 position, Vector3 angles)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            Roster.SetRaycastTargets(true);
            inReposition = false;

            Board.PlaceShip(
                Selection.ThisShip,
                position,
                angles,
                Phases.Next
            );
        }

        public override void Update()
        {
            if (IsLocked) return;

            if (!(RemoteOwner is HumanPlayer)) return;

            if (inReposition) {
                if (CameraScript.InputMouseIsEnabled) PerformDrag();
                if (CameraScript.InputTouchIsEnabled) PerformTouchDragRotate();
                CheckPerformRotation();
                CheckLimits();
            }
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

        private void CheckLimits()
        {
            MovementTemplates.ReturnRangeRulerR2();
            IsPositionWrong = false;

            foreach (BoxCollider collider in Board.BoardTransform.Find("OffTheBoardHolder").GetComponentsInChildren<BoxCollider>())
            {
                float minDistance = float.MaxValue;
                Vector3 nearestPointRemote = Vector3.zero;
                Vector3 nearestPointEdge = Vector3.zero;

                foreach (Vector3 remotePoint in Selection.ThisShip.ShipBase.GetBaseEdges().Values)
                {
                    Vector3 closestPoint = collider.ClosestPoint(remotePoint);
                    float distance = Vector3.Distance(closestPoint, remotePoint);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestPointRemote = remotePoint;
                        nearestPointEdge = closestPoint;
                    }
                }

                if (Vector3.Distance(nearestPointEdge, nearestPointRemote) < MinBoardEdgeDistance)
                {
                    IsPositionWrong = true;
                    MovementTemplates.ShowRangeRulerR2(nearestPointEdge, nearestPointRemote);
                }
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

        public void StartDrag()
        {
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            inReposition = true;

            if (CameraScript.InputTouchIsEnabled)
            {
                // Setup touch handler
                touchObjectPlacementHandler.SetShip(Selection.ThisShip);

                // With touch controls, wait for confirmation before setting the position
                UI.ShowNextButton();
            }

            IsReadyForCommands = true;
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

        public override void ProcessClick()
        {
            if (inReposition && !IsLocked && CameraScript.InputMouseIsEnabled)
            {
                IsLocked = true;
                UI.CallClickNextPhase();
            }
        }

        public bool IsPositionAllowed(GenericShip ship)
        {
            CheckLimits();

            if (IsPositionWrong)
            {
                Messages.ShowErrorToHuman("This remote's position is not valid");
            }

            return !IsPositionWrong;
        }

        public override void NextButton()
        {
            IsLocked = true;

            // Next button is only used for touch controls -- on next, try to confirm ship's position
            if (Selection.ThisShip != null)
            {
                if (IsPositionAllowed(Selection.ThisShip) )
                {
                    StopDrag();
                }
                else
                {
                    IsLocked = false;
                }
            }
        }

        private void StopDrag()
        {
            MovementTemplates.ReturnRangeRulers();

            Roster.SetRaycastTargets(true);
            inReposition = false;

            GameCommand command = SetupSubPhase.GeneratePlaceShipCommand(
                Selection.ThisShip.ShipId,
                Selection.ThisShip.GetPosition(),
                Selection.ThisShip.GetAngles()
            );
            GameMode.CurrentGameMode.ExecuteCommand(command);
        }

    }

}
