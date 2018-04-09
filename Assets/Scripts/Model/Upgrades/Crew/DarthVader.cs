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
			
		public override bool IsAllowedForShip(GenericShip ship)
		{
			return ship.faction == Faction.Imperial;
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

		//Confirmation for Darth Vader Ability
		private void AskToUseVaderAbility(object sender, System.EventArgs e)
		{
			AskToUseAbility(NeverUseByDefault, resolveDarthVaderAbility);
		}
			
		//This method assigns one crit to defender, and two damages to HostShip
		private void resolveDarthVaderAbility(object sender, System.EventArgs e)
		{
			
			Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Crit);

			Triggers.RegisterTrigger ( new Trigger() {
					Name = "Damage from Darth Vader Ability",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = Combat.Defender.Owner.PlayerNo,
					EventHandler = Combat.Defender.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = "Dath Vader",
						DamageType = DamageTypes.CardAbility
					}

				});
			//Delegates flow control to Hostship damage resolution
			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, delegate{ assignDamageRecursivelyToHost(2); });
		}

		//Assigns n damages to HostShip
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
				//Here, Recursive call
				Triggers.ResolveTriggers (TriggerTypes.OnDamageIsDealt, delegate{assignDamageRecursivelyToHost(--totalDamage);});
			} else {
				//No more damage, then return flow control
				DecisionSubPhase.ConfirmDecision();
			}
		}
	}
}
