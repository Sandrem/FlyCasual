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
            Cost = 6;

            UpgradeAbilities.RemoveAll(a => a is IonDamageAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.IonDamageAbilitySE());
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
            DamageSourceEventArgs ionturretDamage = new DamageSourceEventArgs()
            {
                Source = HostShip,
                SourceDescription = "Ion Cannon Turret",
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionturretDamage, Triggers.FinishTrigger);
        }
    }

}


namespace Abilities.SecondEdition
{
    public class IonDamageAbilitySE : IonDamageAbility
    {

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new RotateArcAction(), HostUpgrade);
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(RotateArcAction), HostUpgrade);
        }

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