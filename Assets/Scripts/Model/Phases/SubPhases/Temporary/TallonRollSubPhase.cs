using BoardTools;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubPhases
{
    public class TallonRollSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;
        private const float ANIMATION_SPEED = 700;
        private int direction;
        private bool IsFinished;

        public override void Start()
        {
            Name = "Tallon Roll SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();
            StartTallonRollRotation();
        }

        public override void Update()
        {
            if (!IsFinished)
            {
                float progressLeft = progressTarget - progressCurrent;
                float progressStep = Mathf.Min(Time.deltaTime * ANIMATION_SPEED * Options.AnimationSpeed, progressLeft);
                progressCurrent = progressCurrent + progressStep;

                Selection.ThisShip.RotateAround(Selection.ThisShip.GetCenter(), direction * progressStep);

                float positionY = (progressCurrent < 45) ? progressCurrent : 90 - progressCurrent;
                positionY = positionY / 90;
                Selection.ThisShip.SetHeight(positionY);

                if (progressCurrent == progressTarget)
                {
                    IsFinished = true;
                    EndTallonRollRotation();
                }
            }
        }

        public void StartTallonRollRotation()
        {
            direction = (Selection.ThisShip.AssignedManeuver.Direction == Movement.ManeuverDirection.Left) ? -1 : 1;
            progressCurrent = 0;
            progressTarget = 90;
        }

        private void EndTallonRollRotation()
        {
            GameManagerScript.Instance.StartCoroutine(CheckAwailiblePositions());
        }

        private IEnumerator CheckAwailiblePositions()
        {
            TallonRollHelper tallonRollHelper = new TallonRollHelper(Selection.ThisShip);
            yield return tallonRollHelper.CheckPositions();

            bool isAnyPositionAvailable = tallonRollHelper.IsPositionAllowed.Values.Any(n => n == true);
            if (isAnyPositionAvailable)
            {
                TallonRollShiftSubPhase subPhase = Phases.StartTemporarySubPhaseNew<TallonRollShiftSubPhase>(
                    "Tallon Roll Shift",
                    delegate { Phases.FinishSubPhase(typeof(TallonRollSubPhase)); }
                );

                subPhase.AddDecision("Forward", delegate { TrySelectPosition(tallonRollHelper, 1); }, isCentered: true);
                subPhase.AddDecision("Center", delegate { TrySelectPosition(tallonRollHelper, 0); }, isCentered: true);
                subPhase.AddDecision("Backward", delegate { TrySelectPosition(tallonRollHelper, -1); }, isCentered: true);

                subPhase.DescriptionShort = "Select final position";

                subPhase.DecisionOwner = Selection.ThisShip.Owner;
                subPhase.DefaultDecisionName = "Center";
                subPhase.OnNextButtonIsPressed = FinishTallonRoll;

                subPhase.Start();
            }
            else
            {
                Messages.ShowError("Tallon Roll: No available positions");
            }
        }

        private void FinishTallonRoll()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void TrySelectPosition(TallonRollHelper tallonRollHelper, int direction)
        {
            UI.HideNextButton();

            Selection.ThisShip.SetPosition(tallonRollHelper.GetPosition(direction));

            DecisionSubPhase.ResetInput();

            if (tallonRollHelper.IsPositionAllowed[direction])
            {
                UI.ShowNextButton();
            }
            else
            {
                Messages.ShowError("This position is not available");
            }

            if (Selection.ThisShip.Owner is Players.GenericAiPlayer)
            {
                UI.CallClickNextPhase();
            }
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

    public class TallonRollShiftSubPhase : DecisionSubPhase {}

}

public class TallonRollHelper
{
    private GenericShip Ship { get; }
    private Vector3 PositionInitial { get; }

    private Dictionary<int, GameObject> TemporaryShipBases;
    public Dictionary<int, bool> IsPositionAllowed { get; private set; }

    public TallonRollHelper(GenericShip ship)
    {
        Ship = ship;
        PositionInitial = ship.GetPosition();
    }

    public Vector3 GetPosition(int direction)
    {
        Vector3 shiftDirection = direction * Ship.GetFrontFacing();
        Vector3 shiftAmount = shiftDirection * Board.BoardIntoWorld(Board.DISTANCE_1 / 4f);

        return (PositionInitial + shiftAmount);
    }

    public IEnumerator CheckPositions()
    {
        GeneratTemporaryShipBases();
        yield return CheckCollisions();
        ProcessResults();
        DestroyTemporaryShipBases();
    }

    private void GeneratTemporaryShipBases()
    {
        TemporaryShipBases = new Dictionary<int, GameObject>();

        for (int i = -1; i < 2; i++)
        {
            GameObject prefab = (GameObject)Resources.Load(Ship.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            GameObject temporaryShipBase = MonoBehaviour.Instantiate(
                prefab,
                GetPosition(i),
                Ship.GetRotation(),
                Board.GetBoard()
            );

            foreach (Renderer renderer in temporaryShipBase.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            TemporaryShipBases.Add(i, temporaryShipBase);
        }
    }

    private IEnumerator CheckCollisions()
    {
        foreach (var temporaryShipBase in TemporaryShipBases.Values)
        {
            ObstaclesStayDetectorForced detector = temporaryShipBase.transform.Find("ShipBase").Find("ObstaclesStayDetector").gameObject.AddComponent<ObstaclesStayDetectorForced>();

            detector.ReCheckCollisionsStart();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            detector.ReCheckCollisionsFinish();
        }
    }

    private void ProcessResults()
    {
        IsPositionAllowed = new Dictionary<int, bool>();

        for (int i = -1; i < 2; i++)
        {
            List<GenericShip> overlappedShips = TemporaryShipBases[i].GetComponentInChildren<ObstaclesStayDetectorForced>().OverlappedShipsNow;
            bool isPositionAllowed = !overlappedShips.Where(n => n.ShipId != Ship.ShipId).Any();
            IsPositionAllowed.Add(i, isPositionAllowed);
        }
    }

    private void DestroyTemporaryShipBases()
    {
        foreach (var temporaryShipBase in TemporaryShipBases.Values)
        {
            GameObject.Destroy(temporaryShipBase);
        }
        TemporaryShipBases.Clear();
    }
}
