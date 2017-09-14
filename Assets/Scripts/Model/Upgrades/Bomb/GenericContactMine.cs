using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;

namespace Upgrade
{

    abstract public class GenericContactMine : GenericBomb
    {
        public GenericContactMine() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += PerformDropBombAction;
        }

        private void PerformDropBombAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.BombDropAction()
            {
                Name = "Drop " + Name,
                Source = this
            };
            Host.AddAvailableAction(action);
        }

        public override void ActivateBomb(GameObject bombObject)
        {
            base.ActivateBomb(bombObject);

            BombsManager.RegisterMine(bombObject, this);
        }

        public override void Detonate(object sender, EventArgs e)
        {
            Messages.ShowError("BOOM!!!");

            BombsManager.UnregisterMine(BombObject);
            RegisterDetonationTriggerForShip(sender as GenericShip);

            base.Detonate(sender, e);
        }

        public override void Discard()
        {
            Host.AfterGenerateAvailableActionsList -= PerformDropBombAction;

            base.Discard();
        }

    }

}