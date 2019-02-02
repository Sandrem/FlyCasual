﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;

namespace UpgradesList.FirstEdition
{
    public class ThermalDetonators : GenericTimedBomb
    {
        public ThermalDetonators() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Thermal Detonators",
                type: UpgradeType.Bomb,
                cost: 3
            );

            bombPrefabPath = "Prefabs/Bombs/ThermalDetonator";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            DamageSourceEventArgs thermaldetDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.BombDetonation
            };

            ship.Damage.TryResolveDamage(1, thermaldetDamage, delegate { ship.Tokens.AssignToken(typeof(Tokens.StressToken), callBack); });
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }
    }
}