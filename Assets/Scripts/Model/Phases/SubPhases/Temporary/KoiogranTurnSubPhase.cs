using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class KoiogranTurnSubPhase : GenericSubPhase
    {
        private float progressCurrent;
        private float progressTarget;
        private const float ANIMATION_SPEED = 100;

        public override void Start()
        {
            Name = "Koiogran Turn SubPhase";
            IsTemporary = true;
            UpdateHelpInfo();
            StartKoiogranTurn();
        }

        public void StartKoiogranTurn()
        {
            progressCurrent = 0;
            progressTarget = 180;
        }

        public override void Update()
        {
            float progressStep = Mathf.Min(Time.deltaTime * ANIMATION_SPEED * Options.AnimationSpeed, progressTarget - progressCurrent);
            progressCurrent += progressStep;

            Selection.ThisShip.RotateAround(Selection.ThisShip.GetCenter(), progressStep);

            float positionY = (progressCurrent < 90) ? progressCurrent : 180 - progressCurrent;
            positionY = positionY / 90;
            Selection.ThisShip.SetHeight(positionY);

            if (progressCurrent == progressTarget) EndKoiogranTurn();
        }

        private void EndKoiogranTurn()
        {
            Phases.FinishSubPhase(typeof(KoiogranTurnSubPhase));
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
