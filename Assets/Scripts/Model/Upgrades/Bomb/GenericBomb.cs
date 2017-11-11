using UnityEngine;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Upgrade
{

    abstract public class GenericBomb : GenericUpgrade
    {
        public string bombPrefabPath;
        public bool IsDiscardedAfterDropped;

        public GameObject BombObject { get; private set; }

        public GenericBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
        }

        public virtual void PayDropCost(Action callBack)
        {
            if (IsDiscardedAfterDropped) TryDiscard(callBack);
        }

        public virtual void ActivateBomb(GameObject bombObject, Action callBack)
        {
            BombObject = bombObject;
            Host.IsBombAlreadyDropped = true;
            PayDropCost(callBack);
        }

        public virtual void Detonate(object sender, EventArgs e)
        {
            PlayDetonationAnimSound(ResolveDetonationTriggers);
        }

        private void ResolveDetonationTriggers()
        {
            GameObject.Destroy(BombObject);
            Triggers.ResolveTriggers(TriggerTypes.OnBombDetonated, Triggers.FinishTrigger);
        }

        protected void RegisterDetonationTriggerForShip(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = ship.ShipId + " : Detonation of " + Name,
                TriggerType = TriggerTypes.OnBombDetonated,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = delegate
                {
                    ExplosionEffect(ship, Triggers.FinishTrigger);
                }
            });
        }

        public virtual void ExplosionEffect(GenericShip ship, Action callBack)
        {
            callBack();
        }

        public virtual void PlayDetonationAnimSound(Action callBack)
        {
            callBack();
        }

    }

}
