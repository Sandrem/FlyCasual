using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class GarvenDreis : XWing
        {
            public GarvenDreis() : base()
            {
                PilotName = "Garven Dreis";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/f/f8/Garven-dreis.png";
                IsUnique = true;
                PilotSkill = 6;
                Cost = 26;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnTokenIsSpent += RegisterGarvenDreisPilotAbility;
            }

            private void RegisterGarvenDreisPilotAbility(GenericShip ship, System.Type type)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Garven Dreis' ability",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnTokenIsSpent,
                    EventHandler = StartSubphaseForGarvenDreisPilotAbility
                });
            }

            private void StartSubphaseForGarvenDreisPilotAbility(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                if (Owner.Ships.Count > 1)
                {
                    Phases.StartTemporarySubPhaseOld(
                        "Select target for Garven Dreis' ability",
                        typeof(SubPhases.GarvenDreisAbilityTargetSubPhase),
                        delegate {
                            Phases.CurrentSubPhase.Resume();
                            Triggers.FinishTrigger();
                        }
                    );
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

        }
    }
}

namespace SubPhases
{

    public class GarvenDreisAbilityTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.OtherFriendly);
            maxRange = 2;
            finishAction = SelectGarvenDreisAbilityTarget;

            UI.ShowSkipButton();
        }

        private void SelectGarvenDreisAbilityTarget()
        {
            MovementTemplates.ReturnRangeRuler();

            TargetShip.AssignToken(
                new Tokens.FocusToken(),
                delegate {
                    Phases.FinishSubPhase(typeof(GarvenDreisAbilityTargetSubPhase));
                    CallBack();
                });
        }

        protected override void RevertSubPhase() { }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(GarvenDreisAbilityTargetSubPhase));
            CallBack();
        }

    }

}
