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
        private int updatesCount;
        private ObstaclesStayDetectorForced collisionChecker;
        private Action CallBack;

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

        public override void ActivateBomb(GameObject bombObject, Action callBack)
        {
            CallBack = callBack;

            base.ActivateBomb(bombObject, RegisterMine);
        }

        private void RegisterMine()
        {
            BombsManager.RegisterMine(BombObject, this);
            CheckImmediateDetonation();
        }

        private void CheckImmediateDetonation()
        {
            collisionChecker = BombObject.GetComponentInChildren<ObstaclesStayDetectorForced>();
            collisionChecker.ReCheckCollisionsStart();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.FuncsToUpdate.Add(UpdateColisionDetection);
        }

        private bool UpdateColisionDetection()
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults();
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults()
        {
            collisionChecker.ReCheckCollisionsFinish();

            if (collisionChecker.OverlapsShipNow)
            {
                //TODO: FIX: Select manually
                GenericShip detonatedShip = collisionChecker.OverlappedShipsNow[0];

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Damage from mine",
                    TriggerOwner = detonatedShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnPositionFinish,
                    EventHandler = Detonate,
                    Sender = detonatedShip
                });

                Triggers.ResolveTriggers(TriggerTypes.OnBombDetonated, CallBack);
            }
            else
            {
                CallBack();
            }
        }

        public override void Detonate(object sender, EventArgs e)
        {
            BombsManager.UnregisterMine(BombObject);
            RegisterDetonationTriggerForShip(sender as GenericShip);

            base.Detonate(sender, e);
        }

        public override void Discard(Action callBack)
        {
            Host.AfterGenerateAvailableActionsList -= PerformDropBombAction;

            base.Discard(callBack);
        }

    }

}