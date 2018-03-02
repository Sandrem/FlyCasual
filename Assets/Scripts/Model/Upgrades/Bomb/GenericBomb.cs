using UnityEngine;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using Bombs;

namespace Upgrade
{

    abstract public class GenericBomb : GenericUpgrade
    {
        public string bombPrefabPath;

        public string bombSidePrefabPath;
        public float bombSideDistanceX;
        public float bombSideDistanceZ;

        public bool IsDiscardedAfterDropped;

        public List<GameObject> CurrentBombObjects = new List<GameObject>();

        public GenericBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
        }

        public virtual void PayDropCost(Action callBack)
        {
            if (IsDiscardedAfterDropped)
            {
                TryDiscard(callBack);
            }
            else
            {
                callBack();
            }
        }

        public virtual void ActivateBombs(List<GameObject> bombObjects, Action callBack)
        {
            Host.IsBombAlreadyDropped = true;
            BombsManager.RegisterBombs(bombObjects, this);
            PayDropCost(callBack);
        }

        public virtual void TryDetonate(object sender, EventArgs e)
        {
            BombsManager.CurrentBombObject = (e as BombDetonationEventArgs).BombObject;
            BombsManager.CurrentBomb = BombsManager.GetBombByObject(BombsManager.CurrentBombObject);
            BombsManager.DetonatedShip = (e as BombDetonationEventArgs).DetonatedShip;

            BombsManager.CallGetPermissionToDetonateTrigger(Detonate);
        }

        protected virtual void Detonate()
        {
            BombsManager.UnregisterBomb(BombsManager.CurrentBombObject);
            CurrentBombObjects.Remove(BombsManager.CurrentBombObject);

            PlayDetonationAnimSound(BombsManager.CurrentBombObject, BombsManager.ResolveDetonationTriggers);
        }

        protected void RegisterDetonationTriggerForShip(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = ship.ShipId + " : Detonation of " + Name,
                TriggerType = TriggerTypes.OnBombIsDetonated,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = delegate
                {
                    CheckIgnoreExplosionEffect(ship, Triggers.FinishTrigger);
                }
            });
        }

        private void CheckIgnoreExplosionEffect(GenericShip ship, Action callBack)
        {
            ship.CallCheckSufferBombDetonation(delegate { AfterCheckIgnoreExplosionEffect(ship, callBack); });
        }

        private void AfterCheckIgnoreExplosionEffect(GenericShip ship, Action callBack)
        {
            if (!ship.IgnoressBombDetonationEffect)
            {
                ExplosionEffect(ship, callBack);
            }
            else
            {
                Messages.ShowInfo(string.Format("{0} ignored effect of detonation of {1}", ship.PilotName, BombsManager.CurrentBomb.Name));
                callBack();
            }
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
