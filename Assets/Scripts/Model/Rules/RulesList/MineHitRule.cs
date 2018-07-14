using System.Collections;
using UnityEngine;
using Ship;
using Bombs;

namespace RulesList
{
    public class MineHitRule
    {
        static bool RuleIsInitialized = false;

        public MineHitRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnPositionFinishGlobal += CheckDamage;
                RuleIsInitialized = true;
            }
        }

        private void CheckDamage(GenericShip ship)
        {
            if (ship.MinesHit.Count > 0)
            {
                foreach (var mine in ship.MinesHit)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Damage from mine",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnPositionFinish,
                        EventHandler = BombsManager.GetBombByObject(mine).TryDetonate,
                        EventArgs = new BombDetonationEventArgs()
                        {
                            DetonatedShip = ship,
                            BombObject = mine
                        }
                    });
                }
            }
        }

    }
}