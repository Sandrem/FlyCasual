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

        private void CheckDamage()
        {
            if (Selection.ThisShip.MinesHit.Count > 0)
            {
                foreach (var mine in Selection.ThisShip.MinesHit)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Damage from mine",
                        TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnShipMovementFinish,
                        EventHandler = BombsManager.GetMineByObject(mine).Detonate,
                        Sender = Selection.ThisShip
                    });
                }
            }
        }

    }
}