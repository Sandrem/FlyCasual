using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class KoiogranTurnSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;
        private const float ANIMATION_SPEED = 700;

        public override void Start()
        {
            Name = "Koiogran Turn SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();
            StartRotation();
        }

        public void StartRotation()
        {
            progressCurrent = 0;
            progressTarget = 180;
        }

        public override void Update()
        {
            float progressStep = Mathf.Min(Time.deltaTime * ANIMATION_SPEED * Options.AnimationSpeed, progressTarget - progressCurrent);
            progressCurrent += progressStep;

            Selection.ThisShip.RotateAround(Selection.ThisShip.GetCenter(), progressStep);

            float positionY = (progressCurrent < progressTarget / 2) ? progressCurrent : progressTarget - progressCurrent;
            positionY = positionY / 90;
            Selection.ThisShip.SetHeight(positionY);

            if (progressCurrent == progressTarget) EndRotation();
        }

        private void EndRotation()
        {
            Phases.FinishSubPhase(typeof(KoiogranTurnSubPhase));
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

}
