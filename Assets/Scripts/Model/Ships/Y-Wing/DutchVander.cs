using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class DutchVander : YWing
        {
            public DutchVander() : base()
            {
                PilotName = "\"Dutch\" Vander";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/b/bf/Dutch_Vander.png";
                PilotSkill = 6;
                Cost = 23;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebels;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnTokenIsAssigned += DutchVanderPilotAbility;
            }

            private void DutchVanderPilotAbility(GenericShip ship, System.Type tokenType)
            {
                if (tokenType == typeof(Tokens.BlueTargetLockToken))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "\"Dutch\" Vander: Aquire Tarhet Lock",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnTokenIsAssigned,
                        EventHandler = StartSubphaseForDutchVanderPilotAbility
                    });
                }
            }

            private void StartSubphaseForDutchVanderPilotAbility(object sender, System.EventArgs e)
            {
                Selection.ThisShip = this;
                if (Owner.Ships.Count > 1)
                {
                    Phases.StartTemporarySubPhaseOld(
                        "Select target for \"Dutch\" Vander's ability",
                        typeof(SubPhases.DutchVanderAbilityTargetSubPhase),
                        delegate {
                            Phases.FinishSubPhase(typeof(SubPhases.DutchVanderAbilityTargetSubPhase));
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

    public class DutchVanderAbilityTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isFriendlyAllowed = true;
            maxRange = 2;
            finishAction = SelectGarvenDreisAbilityTarget;

            UI.ShowSkipButton();
        }

        private void SelectGarvenDreisAbilityTarget()
        {
            MovementTemplates.ReturnRangeRuler();

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select target for Target Lock",
                TriggerOwner = TargetShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnAbilityDirect,
                EventHandler = StartSubphaseForTargetLock
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, CallBack);
        }

        protected override void RevertSubPhase() { }

        private void StartSubphaseForTargetLock(object sender, System.EventArgs e)
        {
            Selection.ThisShip = TargetShip;

            Phases.StartTemporarySubPhaseOld(
                "Select target for Target Lock",
                typeof(FreeSelectTargetLockSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(FreeSelectTargetLockSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(SubPhases.DutchVanderAbilityTargetSubPhase));
            Triggers.FinishTrigger();
        }

    }

    public class FreeSelectTargetLockSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isEnemyAllowed = true;
            finishAction = TrySelectTargetLock;

            UI.ShowSkipButton();
        }

        private void TrySelectTargetLock()
        {
            Actions.AssignTargetLockToPair(
                Selection.ThisShip,
                TargetShip,
                delegate {
                    Phases.FinishSubPhase(typeof(FreeSelectTargetLockSubPhase));
                    CallBack();
                },
                RevertSubPhase
            );
        }

        protected override void RevertSubPhase() { }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(FreeSelectTargetLockSubPhase));
            Triggers.FinishTrigger();
        }

    }

}
