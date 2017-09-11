using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            if (IsDropAfterManeuverRevealed) host.OnMovementStart += AskDropBomb;
            if (IsDropAsAction) host.AfterGenerateAvailableActionsList += PerformDropBombAction;
        }

        public virtual void PayDropCost(Action callBack)
        {
            if (IsDiscardedAfterDropped) Discard();

            callBack();
        }

        public virtual void AskDropBomb(GenericShip ship)
        {
            Messages.ShowInfo("Bomb can be dropped");
        }

        public virtual void PerformDropBombAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.DropBombAction();
            action.Name = "Drop " + Name;
            Host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{

    public class DropBombAction : GenericAction
    {
        public DropBombAction()
        {
            Name = EffectName = "Drop Bomb";
        }

        public override void ActionTake()
        {
            Messages.ShowInfo("Bomb is dropped");
            Phases.CurrentSubPhase.CallBack();
        }
    }

}
