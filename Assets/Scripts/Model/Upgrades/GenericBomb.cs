using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;

namespace Upgrade
{

    public class GenericBomb : GenericUpgrade
    {
        public string bombPrefabPath;
        public bool IsDiscardedAfterDropped;
        public bool IsDropAfterManeuverRevealed;
        public bool IsDropAsAction;

        public GenericBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            if (IsDropAfterManeuverRevealed) host.OnMovementStart += RegisterAskDropBomb;
            if (IsDropAsAction) host.AfterGenerateAvailableActionsList += PerformDropBombAction;
        }

        public virtual void PayDropCost(Action callBack)
        {
            if (IsDiscardedAfterDropped) Discard();

            callBack();
        }

        public virtual void RegisterAskDropBomb(GenericShip host)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name + " : Ask to drop",
                TriggerOwner = host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnShipMovementStart,
                EventHandler = AskDropBomb
            });
        }

        public virtual void AskDropBomb(object sender, System.EventArgs e)
        {
            BombsManager.CurrentBombToDrop = this;
            Phases.StartTemporarySubPhase(
                Name,
                typeof(SubPhases.DropBombDecisionSubPhase),
                delegate () { Triggers.FinishTrigger(); }
            );
        }

        public virtual void PerformDropBombAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.BombDropAction();
            action.Name = "Drop " + Name;
            action.Source = this;
            Host.AddAvailableAction(action);
        }

        public void ActivateBomb(GameObject bombObject)
        {
            //TODO: Activate dropped bomb
        }

    }

}

namespace SubPhases
{

    public class DropBombDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Drop " + Phases.CurrentSubPhase.Name + "?";

            AddDecision("Yes", DropBomb);
            AddDecision("No", SkipDropBomb);

            defaultDecision = "No";
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhase(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                ConfirmDecision
            );
        }

        private void SkipDropBomb(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
