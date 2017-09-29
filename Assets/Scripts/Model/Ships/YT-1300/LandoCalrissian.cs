using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class LandoCalrissian : YT1300
        {
            public LandoCalrissian() : base()
            {
                PilotName = "Lando Calrissian";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/YT-1300/lando-calrissian.png";
                PilotSkill = 7;
                Cost = 44;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnMovementExecuted += CheckLandoCalrissianPilotAbility;
            }

            public void CheckLandoCalrissianPilotAbility(GenericShip ship)
            {
                if (ship.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.Green)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Lando Calrissian's ability",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnShipMovementExecuted,
                        EventHandler = LandoCalrissianPilotAbility
                    });
                }
            }

            private void LandoCalrissianPilotAbility(object sender, System.EventArgs e)
            {
                Phases.StartTemporarySubPhase(
                    "Select target for Lando Calrissian's ability",
                    typeof(SubPhases.SelectLandoCalrissianPilotAbilityTargetSubPhase),
                    Triggers.FinishTrigger
                );
            }
        }
    }
}

namespace SubPhases
{

    public class SelectLandoCalrissianPilotAbilityTargetSubPhase : SelectShipSubPhase
    {
        private Ship.GenericShip originalShip;

        public override void Prepare()
        {
            isFriendlyAllowed = true;
            maxRange = 1;
            finishAction = SelectLandoCalrissianPilotAbilityTarget;

            originalShip = Selection.ThisShip;

            UI.ShowSkipButton();
        }

        private void SelectLandoCalrissianPilotAbilityTarget()
        {
            Selection.ThisShip = TargetShip;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Lando Calrissian's ability: Free action",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = PerformFreeAction
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, delegate {
                Phases.FinishSubPhase(typeof(SelectLandoCalrissianPilotAbilityTargetSubPhase));
                Triggers.FinishTrigger();
            });
        }

        protected override void RevertSubPhase() { }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailablePrintedActionsList();

            TargetShip.AskPerformFreeAction(
                actions,
                delegate
                {
                    Selection.ThisShip = originalShip;
                    Triggers.FinishTrigger();
                });
        }

        public override void SkipButton()
        {
            Selection.ThisShip = originalShip;
            Phases.FinishSubPhase(this.GetType());
            Triggers.FinishTrigger();
        }

    }

}
