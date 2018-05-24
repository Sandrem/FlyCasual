using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using ActionsList;

namespace UpgradesList
{

    public class IonCannonTurret : GenericSecondaryWeapon, ISecondEditionUpgrade
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

            UpgradeAbilities.Add(new IonCannonTurretAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/qepSXTj.png";

            UpgradeAbilities.RemoveAll(a => a is IonCannonTurretAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.IonCannonTurretAbility());
            UpgradeAbilities.Add(new GenericActionBarAbility<RotateArcAction>());
        }
    }
}

namespace Abilities
{
    public class IonCannonTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonTurretEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterIonTurretEffect;
        }

        protected void RegisterIonTurretEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
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

        protected virtual void IonTurretEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignToken(
                new Tokens.IonToken(Combat.Defender),
                delegate {
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(2, DefenderSuffersDamage);
                }
            );
        }

        protected void DefenderSuffersDamage()
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


namespace Abilities.SecondEdition
{
    public class IonCannonTurretAbility : Abilities.IonCannonTurretAbility
    {
        protected override void IonTurretEffect(object sender, System.EventArgs e)
        {
            var ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new Tokens.IonToken(Combat.Defender),
                ionTokens,
                delegate {
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(2, DefenderSuffersDamage);
                }
            );
        }
    }

}