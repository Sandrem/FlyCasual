using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Bombs;

namespace UpgradesList
{

    public class ThermalDetonators : GenericTimedBomb
    {

        public ThermalDetonators() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Thermal Detonators";
            Cost = 3;

            bombPrefabPath = "Prefabs/Bombs/ThermalDetonator";

            IsDiscardedAfterDropped = true;
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

            Triggers.ResolveTriggers(
                TriggerTypes.OnDamageIsDealt,
                delegate { ship.Tokens.AssignToken(typeof(Tokens.StressToken), callBack); }
            );
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            BombsManager.CurrentBomb = this;

            Sounds.PlayBombSound(bombObject, "Explosion-7");
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1.4f, delegate { callBack(); });
        }

    }

}
