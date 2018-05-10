using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;
using RuleSets;

namespace UpgradesList
{

    public class SeismicCharges : GenericTimedBomb, ISecondEditionUpgrade
    {

        public SeismicCharges() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Seismic Charges";
            Cost = 2;

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";

            IsDiscardedAfterDropped = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 2;

            ImageUrl = "https://i.imgur.com/kSxLF1I.png";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            ship.AssignedDamageDiceroll.AddDice(DieSide.Success);

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

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1, delegate { PlayDefferedSound(bombObject, callBack); });
        }

        private void PlayDefferedSound(GameObject bombObject, Action callBack)
        {
            Sounds.PlayBombSound(bombObject, "SeismicBomb");
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1.4f, delegate { callBack(); });
        }

    }

}
