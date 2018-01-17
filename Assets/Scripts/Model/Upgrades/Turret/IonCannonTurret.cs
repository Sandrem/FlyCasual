using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class IonCannonTurret : GenericSecondaryWeapon
    {
        public IonCannonTurret() : base()
        {
            Types.Add(UpgradeType.Turret);

            Name = "Ion Cannon Turret";
            Cost = 5;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            CanShootOutsideArc = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            SubscribeOnHit();
        }

        private void SubscribeOnHit()
        {
            Host.OnShotHitAsAttacker += RegisterIonTurretEffect;
        }

        private void RegisterIonTurretEffect()
        {
            if (Combat.ChosenWeapon == this)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ion Cannon Turret effect",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = IonTurretEffect
                });
            }
        }

        private void IonTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.AssignToken(
                new Tokens.IonToken(),
                delegate {
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(2, DefenderSuffersDamage);
                }
            );
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Combat.Defender.Owner.PlayerNo,
                EventHandler = Combat.Defender.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = Combat.Attacker,
                    DamageType = DamageTypes.ShipAttack
                },
                Skippable = true
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }

    }

}