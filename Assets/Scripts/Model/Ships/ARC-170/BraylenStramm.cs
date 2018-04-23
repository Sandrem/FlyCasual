using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System.Linq;
using Abilities;

namespace Ship
{
	namespace ARC170
	{
		public class BraylenStramm : ARC170
		{
			public BraylenStramm() : base()
			{
				PilotName = "Braylen Stramm";
				PilotSkill = 3;
				Cost = 25;

				IsUnique = true;

				PilotAbilities.Add(new Abilities.BraylenStrammPilotAbility());
			}
		}
	}
}

namespace Abilities
{
	public class BraylenStrammPilotAbility : GenericAbility
	{
		public override void ActivateAbility()
		{
			HostShip.OnMovementFinish += RegisterBraylenStrammPilotAbility;
		}

		public override void DeactivateAbility()
		{
			HostShip.OnMovementFinish -= RegisterBraylenStrammPilotAbility;
		}

		private void RegisterBraylenStrammPilotAbility(GenericShip ship)
		{
			RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, BraylenStrammAbility);
		}

		private void BraylenStrammAbility(object sender, System.EventArgs e)
		{
			if (HostShip.Tokens.HasToken (typeof(Tokens.StressToken))) {	
				this.AskToUseAbility(AlwaysUseByDefault, UseBraylenStrammAbility, DontUseBraylenStrammAbility);
			} else {
				//No decision subphase or BraylenStrammCheckSubPhase initiated - simply finish trigger
				Triggers.FinishTrigger();
			}
		}

		private void UseBraylenStrammAbility(object sender, System.EventArgs e)
		{
			Phases.StartTemporarySubPhaseOld(
				"Braylen Stramm Ability: Try to remove stress",
				typeof(SubPhases.BraylenStrammCheckSubPhase),
				delegate {
					//We have a BraylenStrammCheckSubPhase open, so finish it
					Phases.FinishSubPhase(typeof(SubPhases.BraylenStrammCheckSubPhase));

					//We have a Decision SubPhase open, so finish it
					SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

					//The trigger is still active, so finish it.  Must be explicitly finished since ConfirmDecisionNoCallback was used
					Triggers.FinishTrigger();
				}
			);
		}

		private void DontUseBraylenStrammAbility(object sender, System.EventArgs e)
		{
			//We have only a Decision SubPhase open, so finish it
			SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

			//The trigger is still active, so finish it.  Must be explicitly finished since ConfirmDecisionNoCallback was used
			Triggers.FinishTrigger();
		}
	}
}

namespace SubPhases
{
	public class BraylenStrammCheckSubPhase : DiceRollCheckSubPhase
	{
		public override void Prepare()
		{
			diceType = DiceKind.Attack;
			diceCount = 1;

			finishAction = FinishAction;
		}

		protected override void FinishAction()
		{
			HideDiceResultMenu();

			if (CurrentDiceRoll.DiceList [0].Side == DieSide.Success || CurrentDiceRoll.DiceList [0].Side == DieSide.Crit) {
				Messages.ShowInfoToHuman ("Stress is removed");
				this.TheShip.Tokens.RemoveToken (
					typeof(Tokens.StressToken),
					CallBack
				);
			} else {
				CallBack();
			}
		}
	}
}
