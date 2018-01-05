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
        private Action CallBack;

        private int immediateDetonationsCheckedCount;

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

        public override void ActivateBombs(List<GameObject> bombObjects, Action callBack)
        {
            CallBack = callBack;

            base.ActivateBombs(bombObjects, RegisterMine);
        }

        private void RegisterMine()
        {
            BombsManager.RegisterMines(BombObjects, this);
            CheckImmediateDetonation();
        }

        private void CheckImmediateDetonation()
        {
            foreach (var mineObject in BombObjects)
            {
                ObstaclesStayDetectorForced collisionChecker = mineObject.GetComponentInChildren<ObstaclesStayDetectorForced>();
                collisionChecker.ReCheckCollisionsStart();

                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Movement.FuncsToUpdate.Add(delegate { return UpdateColisionDetection(collisionChecker); });
            }
        }

        private bool UpdateColisionDetection(ObstaclesStayDetectorForced collisionChecker)
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults(collisionChecker);
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults(ObstaclesStayDetectorForced collisionChecker)
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
                    EventArgs = new BombDetonationEventArgs()
                    {
                        DetonatedShip = detonatedShip,
                        BombObject = collisionChecker.transform.parent.gameObject
                    }
                });
            }

            immediateDetonationsCheckedCount++;
            int minesDropped = string.IsNullOrEmpty(bombSidePrefabPath) ? 1 : 3;
            if (immediateDetonationsCheckedCount == minesDropped)
            {
                immediateDetonationsCheckedCount = 0;
                Triggers.ResolveTriggers(TriggerTypes.OnBombDetonated, CallBack);
            }
        }

        public override void Detonate(object sender, EventArgs e)
        {
            BombsManager.UnregisterMine((e as BombDetonationEventArgs).BombObject);
            RegisterDetonationTriggerForShip((e as BombDetonationEventArgs).DetonatedShip);

            base.Detonate(sender, e);
        }

        public override void Discard(Action callBack)
        {
            Host.AfterGenerateAvailableActionsList -= PerformDropBombAction;

            base.Discard(callBack);
        }

    }

}