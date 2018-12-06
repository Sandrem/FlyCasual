using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Bombs;

namespace UpgradesList.FirstEdition
{
    public class SeismicCharges : GenericTimedBomb
    {
        public SeismicCharges() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seismic Charges",
                type: UpgradeType.Bomb,
                cost: 2
            );

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            DamageSourceEventArgs seismicDamage = new DamageSourceEventArgs()
            {
                Source = this,
                DamageType = DamageTypes.BombDetonation
            };

            ship.Damage.TryResolveDamage(1, seismicDamage, callBack);
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1, delegate { PlayDefferedSound(bombObject, callBack); });
        }

        private void PlayDefferedSound(GameObject bombObject, Action callBack)
        {
            Sounds.PlayBombSound(bombObject, "SeismicBomb");
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }
    }
}