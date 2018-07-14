using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using ActionsList;
using Tokens;

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

            UpgradeAbilities.Add(new IonDamageAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            UpgradeAbilities.RemoveAll(a => a is IonDamageAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.IonDamageAbilitySE());
            UpgradeAbilities.Add(new GenericActionBarAbility<RotateArcAction>());
        }
    }
}

namespace Abilities
{
    public class IonDamageAbility : GenericAbility
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
                typeof(IonToken),
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
    public class IonDamageAbilitySE : IonDamageAbility
    {
        protected override void IonTurretEffect(object sender, System.EventArgs e)
        {
            var ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            if (ionTokens > 0)
            {
                Combat.Defender.Tokens.AssignTokens(
                    () => new Tokens.IonToken(Combat.Defender),
                    ionTokens,
                    delegate {
                        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                        Game.Wait(2, DefenderSuffersDamage);
                    }
                );
            }
            else
            {
                DefenderSuffersDamage();
            }
        }
    }

}