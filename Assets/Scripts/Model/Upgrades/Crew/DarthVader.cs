using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using System;

namespace UpgradesList
{
	public class DarthVader : GenericUpgrade
	{
		public DarthVader() : base()
		{
			Types.Add(UpgradeType.Crew);
			Name = "Darth Vader";
			Cost = 3;

			UpgradeAbilities.Add(new DarthVaderCrewAbility());
		}
	}
}

namespace Abilities
{
	public class DarthVaderCrewAbility: GenericAbility
	{

		public override void ActivateAbility()
		{
			HostShip.OnAttackFinishAsAttacker += RegisterVaderAbility;
		}


		public override void DeactivateAbility()
		{
			HostShip.OnAttackFinishAsAttacker -= RegisterVaderAbility;
		}


		private void RegisterVaderAbility(GenericShip ship)
		{
			RegisterAbilityTrigger (TriggerTypes.OnAttackFinishAsAttacker, AskToUseVaderAbility);
		}



		private void AskToUseVaderAbility(object sender, System.EventArgs e)
		{
			AskToUseAbility(NeverUseByDefault, defenderSufferDamage);
		}
			

		private void assignDamageRecursivelyToHost (int totalDamage){

			if (totalDamage > 0) {
				HostShip.AssignedDamageDiceroll.AddDice (DieSide.Success);

				Triggers.RegisterTrigger (new Trigger () {
					Name = "Damage from Darth Vader Ability",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Attacker.Owner.PlayerNo,
					EventHandler = Combat.Attacker.SufferDamage,
					EventArgs = new DamageSourceEventArgs () {
						Source = "Dath Vader",
						DamageType = DamageTypes.CardAbility
					}

				}
				);
				//totalDamage--;
				Triggers.ResolveTriggers (TriggerTypes.OnDamageIsDealt, delegate{assignDamageRecursivelyToHost(--totalDamage);});
			} else {
				DecisionSubPhase.ConfirmDecision();
			}
		}


		private void darthVaderAbility(object sender, System.EventArgs e){

			Messages.ShowInfo("Darth Vader used!");

			HostShip.AssignedDamageDiceroll.AddDice (DieSide.Success);
			HostShip.AssignedDamageDiceroll.AddDice (DieSide.Success);

			Triggers.RegisterTrigger ( new Trigger() 
				{
					Name = "Damage from Darth Vader Ability",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Attacker.Owner.PlayerNo,
					EventHandler = Combat.Attacker.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = "Dath Vader",
						DamageType = DamageTypes.CardAbility
					}

				}
			);
			//Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, delegate{defenderSufferDamage();} );
		}
			

		private void defenderSufferDamage(object sender, System.EventArgs e)
		{
			
			Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Crit);

			Triggers.RegisterTrigger ( new Trigger() 
				{
					Name = "Damage from Darth Vader Ability",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Defender.Owner.PlayerNo,
					EventHandler = Combat.Defender.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = "Dath Vader",
						DamageType = DamageTypes.CardAbility
					}

				}
			);
			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, delegate{ assignDamageRecursivelyToHost(2); });
		}
	}
}
