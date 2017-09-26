using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class TallonRollSubPhase : GenericSubPhase
    {
        private bool inProgress;
        private float progressCurrent;
        private float progressTarget;
        private const float ANIMATION_SPEED = 100;
        private int direction;

        public override void Start()
        {
            Name = "Tallon Roll SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            StartTallonRoll();
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

            if (progressCurrent == progressTarget) EndTallonRoll();
        }

        public void StartTallonRoll()
        {
            direction = (Selection.ThisShip.AssignedManeuver.Direction == Movement.ManeuverDirection.Left) ? -1 : 1;
            progressCurrent = 0;
            progressTarget = 90;
            inProgress = true;
        }

        private void EndTallonRoll()
        {
            inProgress = false;
            Phases.FinishSubPhase(typeof(TallonRollSubPhase));
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();

            CallBack();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
