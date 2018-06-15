using System.Collections;
using UnityEngine;
using Ship;
using Bombs;

namespace RulesList
{
    public class MineHitRule
    {

        public MineHitRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GenericShip.OnPositionFinishGlobal += CheckDamage;
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