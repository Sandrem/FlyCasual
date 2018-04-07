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
				//this.AskToUseAbility(delegate {return false;}, UseBraylenStrammAbility, DontUseBraylenStrammAbility);
				UseBraylenStrammAbility (sender, e);
			} else {
				DontUseBraylenStrammAbility (sender, e);
			}
		}

		private void UseBraylenStrammAbility(object sender, System.EventArgs e)
		{
			Phases.StartTemporarySubPhaseOld(
				"Braylen Stramm Ability: Try to remove stress",
				typeof(SubPhases.BraylenStrammCheckSubPhase),
				delegate {
					Phases.FinishSubPhase(typeof(SubPhases.BraylenStrammCheckSubPhase));
					Triggers.FinishTrigger();
				}
			);
		}

		private void DontUseBraylenStrammAbility(object sender, System.EventArgs e)
		{
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
					FinishPhase
				);
			} else {
				FinishPhase ();
			}

			CallBack();
		}
	}
}
