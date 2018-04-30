using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Board;
using GameModes;

namespace SubPhases
{

    public class SetupSubPhase : GenericSubPhase
    {
        private bool inReposition;

        private Transform StartingZone;
        private bool isInsideStartingZone;

        public override void Start()
        {
            base.Start();
            Name = "Setup SubPhase";
        }

        public override void Prepare()
        {
            RequiredPilotSkill = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Phases.FinishSubPhase(typeof(SetupSubPhase));
        }

        public override void Next()
        {
            bool success = GetNextActivation(RequiredPilotSkill);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredPilotSkill);
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
                Roster.GetPlayer(RequiredPlayer).SetupShip();
            }
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.PilotSkill == pilotSkill
                where n.Value.IsSetupPerformed == false
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredPilotSkill = pilotSkill;

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
                where !n.Value.IsSetupPerformed && n.Value.PilotSkill > pilotSkillMin
                orderby n.Value.PilotSkill
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.First().Value.PilotSkill;
            }

            return result;
        }

        public override void FinishPhase()
        {
            BoardManager.TurnOffStartingZones();
            Phases.NextPhase();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (ship.IsSetupPerformed == false)
                {
                    result = true;
                }
                else
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected: Starting position is already set");
                }
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + Phases.CurrentSubPhase.RequiredPlayer + " and pilot skill " + Phases.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        private bool FilterShipsToSetup(GenericShip ship)
        {
            return ship.PilotSkill == RequiredPilotSkill && !ship.IsSetupPerformed && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
        {
            Roster.SetRaycastTargets(true);
            inReposition = false;

            Selection.ChangeActiveShip("ShipId:" + shipId);
            BoardManager.PlaceShip(Selection.ThisShip, position, angles, delegate { Selection.DeselectThisShip(); Phases.Next(); });
        }

        public override void Update()
        {
            if (inReposition) PerformDrag();
            CheckPerformRotation();
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

        private static void RotateBy1()
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
            StartingZone = BoardManager.GetStartingZone(Phases.CurrentSubPhase.RequiredPlayer);
            isInsideStartingZone = false;
            Roster.SetRaycastTargets(false);
            Roster.AllShipsHighlightOff();
            BoardManager.HighlightStartingZones();
            Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().checkCollisions = true;
            inReposition = true;
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
                        float halfOfShipStandSize = BoardManager.BoardIntoWorld(BoardManager.DISTANCE_1 / 2f);
                        float oneOfShipStandSize = BoardManager.BoardIntoWorld(BoardManager.DISTANCE_1);

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
            if (inReposition) TryConfirmPosition(Selection.ThisShip);
        }

        public bool TryConfirmPosition(GenericShip ship)
        {
            bool result = true;

            if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.SetupSubPhase))
            {
                if (!ship.ShipBase.IsInside(StartingZone))

                {
                    Messages.ShowErrorToHuman("Place ship into highlighted area");
                    result = false;
                }

                if (Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().OverlapedShips.Count > 0)
                {
                    Messages.ShowErrorToHuman("This ship shouldn't collide with another ships");
                    result = false;
                }

            }

            if (result) StopDrag();

            return result;
        }

        private void StopDrag()
        {
            HideSetupHelpers();
            Roster.SetRaycastTargets(true);
            Selection.ThisShip.Model.GetComponentInChildren<ObstaclesStayDetector>().checkCollisions = false;
            inReposition = false;

            GameMode.CurrentGameMode.ConfirmShipSetup(Selection.ThisShip.ShipId, Selection.ThisShip.GetPosition(), Selection.ThisShip.GetAngles());
        }

    }

}
