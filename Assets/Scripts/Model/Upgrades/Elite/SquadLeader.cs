using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class SquadLeader : GenericUpgrade
    {

        public SquadLeader() : base()
        {
            Type = UpgradeType.Elite;
            Name = ShortName = "Squad Leader";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/c/cd/Squad_Leader.png";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += SquadLeaderAddAction;
        }

        private void SquadLeaderAddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.SquadLeaderAction() { ImageUrl = ImageUrl };
            host.AddAvailableAction(newAction);
        }

    }

}

namespace ActionsList
{

    public class SquadLeaderAction : GenericAction
    {

        public SquadLeaderAction()
        {
            Name = EffectName = "Squad Leader";
        }

        public override void ActionTake()
        {
            Phases.StartTemporarySubPhase(
                "Select target for Squad Leader",
                typeof(SubPhases.SelectSquadLeaderTargetSubPhase),
                delegate {}
            );
        }

    }

}

namespace SubPhases
{

    public class SelectSquadLeaderTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isFriendlyAllowed = true;
            maxRange = 2;
            finishAction = SelectSquadLeaderTarget;

            UI.ShowSkipButton();
        }

        private void SelectSquadLeaderTarget()
        {
            Selection.ThisShip = TargetShip;

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Squad Leader: Free action",
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFreeActionPlanned,
                    EventHandler = PerformFreeAction
                }
            );

            MovementTemplates.ReturnRangeRuler();

            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, delegate {
                Phases.FinishSubPhase(typeof(SelectSquadLeaderTargetSubPhase));
                Phases.FinishSubPhase(typeof(ActionDecisonSubPhase));
                Triggers.FinishTrigger();
                CallBack();
            });
        }

        protected override void RevertSubPhase() { }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            Selection.ThisShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            TargetShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
