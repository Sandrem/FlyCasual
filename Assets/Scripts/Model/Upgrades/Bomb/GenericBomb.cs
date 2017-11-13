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

        public string bombSidePrefabPath;
        public float bombSideDistanceX;
        public float bombSideDistanceZ;

        public bool IsDiscardedAfterDropped;

        public List<GameObject> BombObjects = new List<GameObject>();

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

        public virtual void ActivateBombs(List<GameObject> bombObjects, Action callBack)
        {
            BombObjects = bombObjects;
            Host.IsBombAlreadyDropped = true;
            PayDropCost(callBack);
        }

        public virtual void Detonate(object sender, EventArgs e)
        {
            GameObject bombObject = (e as Bombs.BombDetonationEventArgs).BombObject;
            PlayDetonationAnimSound(bombObject, delegate { ResolveDetonationTriggers(bombObject); });
        }

        private void ResolveDetonationTriggers(GameObject bombObject)
        {
            GameObject.Destroy(bombObject);
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

        public virtual void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            callBack();
        }

    }

}
