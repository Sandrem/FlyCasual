using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Tokens;
using UnityEngine;
using Upgrade;
using RuleSets;

namespace UpgradesList
{

    public class IonCannon : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public IonCannon()
        {
            Types.Add(UpgradeType.Cannon);

            Name = "Ion Cannon";
            Cost = 3;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            UpgradeAbilities.Add(new IonCannonAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 5;

            UpgradeAbilities.RemoveAll(a => a is IonCannonAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.IonCannonAbilitySE());

            SEImageNumber = 28;
        }
    }
}

namespace Abilities
{
    public class IonCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterIonCannonEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterIonCannonEffect;
        }

        private void RegisterIonCannonEffect()
		{
			if (Combat.ChosenWeapon == HostUpgrade)
			{
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, IonCannonEffect);
			}
		}

		protected virtual void IonCannonEffect(object sender, System.EventArgs e)
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

			Combat.Defender.Tokens.AssignToken(
                typeof(IonToken),
				delegate {
					GameManagerScript.Wait(2, DefenderSuffersDamage);
				}
			);
		}

        protected void DefenderSuffersDamage()
        {
            DamageSourceEventArgs ionCannonDamage = new DamageSourceEventArgs()
            {
                Source = Combat.Attacker,
                DamageType = DamageTypes.ShipAttack
            };

            Combat.Defender.Damage.TryResolveDamage(1, ionCannonDamage, Triggers.FinishTrigger);
        }
    }

}

namespace Abilities.SecondEdition
{
    public class IonCannonAbilitySE : IonCannonAbility
    {

        protected override void IonCannonEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new IonToken(Combat.Defender),
                ionTokens,
                delegate
                {
                    GameManagerScript.Wait(2, DefenderSuffersDamage);
                }
            );
        }
    }
}