using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// Correct work with combat subphase
// What if there is no another frienly ships
// What if I do not want assign token
// What revert should be done if selected ship does not fulfill all requirements
// What revert should be done if selected ship cannot get target lock on first target

namespace Ship
{
    namespace YWing
    {
        public class DutchVander : YWing
        {
            public DutchVander() : base()
            {
                IsHidden = true;

                PilotName = "\"Dutch\" Vander";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/bf/Dutch_Vander.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 23;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterTokenIsAssigned += GarvenDreisPilotAbility;
            }

            private void GarvenDreisPilotAbility(GenericShip ship, System.Type type)
            {
                if (type == typeof(Tokens.BlueTargetLockToken))
                {
                    if (ship.Owner.Ships.Count > 1)
                    {
                        Selection.ActiveShip = ship;
                        Phases.StartTemporarySubPhase("Choose another friendly ship at range 1-2, it will acquire target lock", typeof(SubPhases.DutchVanderAbilitySubPhase));
                    }
                }
            }

        }
    }
}

namespace SubPhases
{

    public class DutchVanderAbilitySubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isFriendlyAllowed = true;
            maxRange = 2;

            finishAction = AcquireTargetLock;

            Game.UI.ShowSkipButton();
        }

        public override void Next()
        {
            Game.UI.HideNextButton();
            PreviousSubPhase.Resume();
            base.Next();
        }

        private void AcquireTargetLock()
        {
            Selection.ThisShip = TargetShip;
            Selection.ActiveShip = TargetShip;
            /*Phases.CancelScheduledFinish(typeof(SelectTargetLockSubPhase));
            Phases.StartTemporarySubPhase("Select target for Target Lock", typeof(SelectTargetLockSubPhase));*/
        }

        protected override void RevertSubPhase()
        {
            Game.UI.HighlightNextButton();
        }

    }

}
