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

		private void IonCannonEffect(object sender, System.EventArgs e)
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

		private void DefenderSuffersDamage()
		{
			Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            var trigger = RegisterAbilityTrigger(TriggerTypes.OnDamageIsDealt, Combat.Defender.SufferDamage, new DamageSourceEventArgs()
			{
				Source = Combat.Attacker,
				DamageType = DamageTypes.ShipAttack
			});
            trigger.Skippable = true;

			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
		}

    }

}

namespace Abilities.SecondEdition
{
    public class IonCannonAbilitySE : IonCannonAbility
    {

        protected void IonCannonEffect(object sender, System.EventArgs e)
        {
            int ionTokens = Combat.DiceRollAttack.Successes - 1;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new IonToken(Combat.Defender),
                ionTokens,
                delegate
                {
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(2, DefenderSuffersDamage);
                }
            );
        }

        private void DefenderSuffersDamage()
        {
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            var trigger = RegisterAbilityTrigger(TriggerTypes.OnDamageIsDealt, Combat.Defender.SufferDamage, new DamageSourceEventArgs()
            {
                Source = Combat.Attacker,
                DamageType = DamageTypes.ShipAttack
            });
            trigger.Skippable = true;

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Triggers.FinishTrigger);
        }
    }
}