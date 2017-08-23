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
            Type = UpgradeSlot.Turret;

            Name = "Ion Cannon Turret";
            ShortName = "Ion Cannon Turret";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/f/f3/Ion_Cannon_Turret.png";
            Cost = 5;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            IsTurret = true;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            SubscribeOnHit();
        }

        private void SubscribeOnHit()
        {
            Host.OnAttackHitAsAttacker += RegisterIonTurretEffect;
        }

        private void RegisterIonTurretEffect()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Ion Cannon Turret effect",
                TriggerType = TriggerTypes.OnAttackHit,
                TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                EventHandler = IonTurretEffect
            });
        }

        private void IonTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.AssignToken(
                new Tokens.IonToken(),
                delegate {
                    DefenderSuffersDamage();
                }
            );
        }

        private void DefenderSuffersDamage()
        {
            Debug.Log(Combat.DiceRollAttack.Successes);

            Combat.Defender.AssignedDamageDiceroll.DiceList.Add(new Dice(DiceKind.Attack, DiceSide.Success));

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = Combat.Defender.Owner.PlayerNo,
                EventHandler = Combat.Defender.SufferDamage,
                Skippable = true
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }

    }

}