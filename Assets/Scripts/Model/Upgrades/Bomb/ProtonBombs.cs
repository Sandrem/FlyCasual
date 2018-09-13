﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;
using RuleSets;

namespace UpgradesList
{

    public class ProtonBombs : GenericTimedBomb, ISecondEditionUpgrade
    {
        GenericShip _ship = null;

        public ProtonBombs() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Proton Bombs";
            Cost = 5;

            bombPrefabPath = "Prefabs/Bombs/ProtonBomb";

            IsDiscardedAfterDropped = true;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            IsDiscardedAfterDropped = false;
            UsesCharges = true;

            MaxCharges = 2;
            Cost = 5;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            _ship = ship;

            if (RuleSet.Instance is FirstEdition)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer damage from bomb",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = DetonationFE,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = this,
                        DamageType = DamageTypes.BombDetonation
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
            }
            else if (RuleSet.Instance is SecondEdition)
            {
                DamageSourceEventArgs protonDamage = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                };

                _ship.Damage.TryResolveDamage(0, 1, protonDamage, callBack);
            }
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(1.4f, delegate { callBack(); });
        }

        private void DetonationFE(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("{0}: Dealt faceup card to {1}", Name, _ship.PilotName));
            _ship.SufferHullDamage(true, e);
        }

    }

}