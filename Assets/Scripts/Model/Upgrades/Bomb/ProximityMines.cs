using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;

namespace UpgradesList
{

    public class ProximityMines : GenericContactMine
    {

        public ProximityMines() : base()
        {
            Type = UpgradeType.Bomb;
            Name = ShortName = "Proximity Mines";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Bomb/proximity-mines.png";
            Cost = 3;

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            //Roll 3 dice - suffer all results

            ship.AssignedDamageDiceroll.AddDice(DiceSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = ship.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

    }

}
