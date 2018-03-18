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
            CurrentBombObjects = bombObjects;

            base.ActivateBombs(bombObjects, CheckImmediateDetonation);
        }

        private void CheckImmediateDetonation()
        {
            foreach (var mineObject in CurrentBombObjects)
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
                    EventHandler = TryDetonate,
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
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

                for (int i = 0; i < minesDropped; i++)
                {
                    Game.Movement.FuncsToUpdate.Remove(delegate { return UpdateColisionDetection(collisionChecker); });
                }

                immediateDetonationsCheckedCount = 0;
                Triggers.ResolveTriggers(TriggerTypes.OnBombIsDetonated, CallBack);
            }
        }

        protected override void Detonate()
        {
            RegisterDetonationTriggerForShip(BombsManager.DetonatedShip);

            base.Detonate();
        }

        public override void Discard(Action callBack)
        {
            Host.AfterGenerateAvailableActionsList -= PerformDropBombAction;

            base.Discard(callBack);
        }

    }

}