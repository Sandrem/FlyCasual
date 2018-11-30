using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;

namespace UpgradesList.SecondEdition
{
    public class ProtonBombs : GenericTimedBomb
    {
        GenericShip _ship = null;

        public ProtonBombs() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Bombs",
                type: UpgradeType.Bomb,
                cost: 5,
                charges: 2,
                seImageNumber: 65
            );

            bombPrefabPath = "Prefabs/Bombs/ProtonBomb";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            _ship = ship;


            DamageSourceEventArgs protonDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.BombDetonation
            };

            _ship.Damage.TryResolveDamage(0, 1, protonDamage, callBack);
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