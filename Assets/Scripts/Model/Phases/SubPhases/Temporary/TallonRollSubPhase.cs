using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class TallonRollSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;
        private const float ANIMATION_SPEED = 700;
        private int direction;

        private Vector3 PositionAfterRotation { get; set; }

        public override void Start()
        {
            Name = "Tallon Roll SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();
            StartTallonRollRotation();
        }

        public override void Update()
        {
            float progressLeft = progressTarget - progressCurrent;
            float progressStep = Mathf.Min(Time.deltaTime * ANIMATION_SPEED * Options.AnimationSpeed, progressLeft);
            progressCurrent = progressCurrent + progressStep;

            Selection.ThisShip.RotateAround(Selection.ThisShip.GetCenter(), direction*progressStep);

            float positionY = (progressCurrent < 45) ? progressCurrent : 90 - progressCurrent;
            positionY = positionY / 90;
            Selection.ThisShip.SetHeight(positionY);

            if (progressCurrent == progressTarget) EndTallonRollRotation();
        }

        public void StartTallonRollRotation()
        {
            direction = (Selection.ThisShip.AssignedManeuver.Direction == Movement.ManeuverDirection.Left) ? -1 : 1;
            progressCurrent = 0;
            progressTarget = 90;
        }

        private void EndTallonRollRotation()
        {
            PositionAfterRotation = Selection.ThisShip.GetPosition();

            TallonRollShiftSubPhase subPhase = Phases.StartTemporarySubPhaseNew<TallonRollShiftSubPhase>(
                "Tallon Roll Shift",
                delegate { Phases.FinishSubPhase(typeof(TallonRollSubPhase)); }
            );

            subPhase.AddDecision("Forward", delegate { ConfirmDecision(1); }, isCentered: true);
            subPhase.AddDecision("Center", delegate { ConfirmDecision(0); }, isCentered: true);
            subPhase.AddDecision("Backwards", delegate { ConfirmDecision(-1); }, isCentered: true);

            subPhase.DescriptionShort = "Select final position";

            subPhase.DecisionOwner = Selection.ThisShip.Owner;
            subPhase.DefaultDecisionName = "Center";
            subPhase.OnNextButtonIsPressed = FinishTallonRoll;

            subPhase.Start();
        }

        private void FinishTallonRoll()
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void ConfirmDecision(int direction)
        {
            Vector3 shiftDirection = direction * Selection.ThisShip.GetFrontFacing();
            Vector3 shiftAmount = shiftDirection * BoardTools.Board.BoardIntoWorld(BoardTools.Board.DISTANCE_1 / 4f);

            Selection.ThisShip.SetPosition(PositionAfterRotation + shiftAmount);

            DecisionSubPhase.ResetInput();

            UI.ShowNextButton();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

    public class TallonRollShiftSubPhase : DecisionSubPhase {}

}
