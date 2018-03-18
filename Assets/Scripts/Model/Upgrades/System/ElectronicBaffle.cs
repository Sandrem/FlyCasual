using Upgrade;
using Abilities;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;

namespace UpgradesList
{
	public class ElectronicBaffle : GenericUpgrade
	{
		public ElectronicBaffle() : base()
		{
			Types.Add(UpgradeType.System);
			Name = "Electronic Baffle";
			Cost = 1;

			UpgradeAbilities.Add (new ElectronicBaffleAbility ());
		}
	}
}

namespace Abilities
{

	public class ElectronicBaffleAbility : GenericAbility
	{

		//This ability is gonna be checked on each token assigned to HostShip
		public override void ActivateAbility()
		{
			HostShip.OnTokenIsAssigned += RegisterElectronicBaffle;
		}

		//We're gonna hold this ability during all the round
		public override void DeactivateAbility()
		{

			Phases.OnRoundEnd += RemoveElectronicBuffer;
		}


		private void RemoveElectronicBuffer()
		{
			HostShip.OnTokenIsAssigned -= RegisterElectronicBaffle;
			Phases.OnRoundEnd -= RemoveElectronicBuffer;
		}


		private void RegisterElectronicBaffle(object sender,  System.Type tokenType)
		{
			
			if (tokenType == typeof(Tokens.StressToken)) {
				RegisterAbilityTrigger (TriggerTypes.OnTokenIsAssigned, ShowUseEBStress);
			}

			if (tokenType == typeof(Tokens.IonToken) ){
				RegisterAbilityTrigger (TriggerTypes.OnTokenIsAssigned, ShowUseEBIon);
			}

		}


		private void ShowUseEBIon(object sender, System.EventArgs e)
		{
			AskToUseAbility (AlwaysUseByDefault, RemoveIon);
		}


		private void ShowUseEBStress(object sender, System.EventArgs e)
		{
			AskToUseAbility (AlwaysUseByDefault, RemoveStress);
		}


		private void RemoveIon(object sender, System.EventArgs e)
		{
			HostShip.Tokens.RemoveToken (typeof(Tokens.IonToken), SelectShipSubPhase.FinishSelectionNoCallback);
			sufferDamage ();
		}

		private void RemoveStress(object sender, System.EventArgs e)
		{
			HostShip.Tokens.RemoveToken (typeof(Tokens.StressToken), SelectShipSubPhase.FinishSelectionNoCallback);
			sufferDamage ();
		}

		private void sufferDamage(){
			
			Triggers.RegisterTrigger(new Trigger()
				{
					Name = "Suffer damage from Electronic Baffle",
					TriggerType = TriggerTypes.OnDamageIsDealt,
					TriggerOwner = HostShip.Owner.PlayerNo,
					EventHandler = HostShip.SufferDamage,
					EventArgs = new DamageSourceEventArgs()
					{
						Source = "ElectronicBaffle",
						DamageType = DamageTypes.CardAbility
					},
					Skippable = true
				});
			Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, SelectShipSubPhase.FinishSelectionNoCallback);
		}
	}
		

}