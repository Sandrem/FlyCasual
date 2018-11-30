using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using Bombs;

namespace UpgradesList.FirstEdition
{
    public class ProtonBombs : GenericTimedBomb
    {
        GenericShip _ship = null;

        public ProtonBombs() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Bombs",
                type: UpgradeType.Bomb,
                cost: 5
            );

            bombPrefabPath = "Prefabs/Bombs/ProtonBomb";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            _ship = ship;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = Detonation,
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

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }

        private void Detonation(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("{0}: Dealt faceup card to {1}", UpgradeInfo.Name, _ship.PilotName));
            _ship.SufferHullDamage(true, e);
        }
    }
}