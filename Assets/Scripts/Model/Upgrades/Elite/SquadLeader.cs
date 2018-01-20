using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using SubPhases;
using Ship;

namespace UpgradesList
{

    public class SquadLeader : GenericUpgrade
    {

        public SquadLeader() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Squad Leader";
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
            ActionsList.GenericAction newAction = new ActionsList.SquadLeaderAction() { ImageUrl = ImageUrl, Host = this.Host };
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
            SelectSquadLeaderTargetSubPhase newPhase = (SelectSquadLeaderTargetSubPhase) Phases.StartTemporarySubPhaseNew(
                "Select target for Squad Leader",
                typeof(SelectSquadLeaderTargetSubPhase),
                delegate {}
            );
            newPhase.SquadLeaderOwner = this.Host;
            newPhase.Start();
        }

    }

}

namespace SubPhases
{

    public class SelectSquadLeaderTargetSubPhase : SelectShipSubPhase
    {
        public GenericShip SquadLeaderOwner;

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.OtherFriendly);
            maxRange = 2;
            finishAction = SelectSquadLeaderTarget;

            UI.ShowSkipButton();
        }

        private void SelectSquadLeaderTarget()
        {
            if (TargetShip.PilotSkill < SquadLeaderOwner.PilotSkill)
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
            else
            {
                Messages.ShowInfo("Target must have lower pilot skill than owner of Squad Leader");
            }
        }

        public override void RevertSubPhase() { }

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
