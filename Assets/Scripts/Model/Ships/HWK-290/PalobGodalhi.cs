using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;
using Players;
using System.Linq;

namespace Ship
{
	namespace HWK290
	{
		public class PalobGodalhi : HWK290
		{
			public PalobGodalhi() : base()
			{
				PilotName = "Palob Godalhi";
				PilotSkill = 5;
				Cost = 20;

				IsUnique = true;

				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

				faction = Faction.Scum;

				PilotAbilities.Add(new Abilities.PalobGodalhi());
			}
		}
	}
}

namespace Abilities
{
	public class PalobGodalhi : GenericAbility
	{
		public override void ActivateAbility()
		{
			Phases.OnCombatPhaseStart_Triggers += RegisterAbility;
		}

		public override void DeactivateAbility()
		{
			Phases.OnCombatPhaseStart_Triggers -= RegisterAbility;
		}

		private void RegisterAbility()
		{
			RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
		}

		private void Ability(object sender, EventArgs e)
		{
			if (TargetsForAbilityExist (FilterAbilityTarget)) {
				Messages.ShowInfoToHuman ("Palob Godalhi: Select a ship to remove Focus/Evade token from");

				SelectTargetForAbility (
					SelectAbilityTarget,
					FilterAbilityTarget,
					GetAiAbilityPriority,
					HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    "Choose a ship to remove 1 focus or evade token from it and assign this token to yourself.",
                    HostShip.ImageUrl
                );
			} else {
					Triggers.FinishTrigger();
			}
		}

		private bool FilterAbilityTarget(GenericShip ship)
		{
			return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 2) && FilterTargetWithTokens(ship);
		}

		private bool FilterTargetWithTokens(GenericShip ship)
		{
			return (ship.Tokens.HasToken(typeof(Tokens.FocusToken)) || ship.Tokens.HasToken(typeof(Tokens.EvadeToken)));
		}

		private int GetAiAbilityPriority(GenericShip ship)
		{
			int result = 0;

			int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(Tokens.FocusToken));
			int shipEvadeTokens = ship.Tokens.CountTokensByType(typeof(Tokens.EvadeToken));

			result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);
			if (shipFocusTokens > 0)
				result += 50;
			if (shipFocusTokens == 1) 
				result += 100;
			if (shipEvadeTokens > 0)
				result += 25;

			return result;
		}

		private void SelectAbilityTarget()
		{
			GenericShip thisship = TargetShip;
			int numfocustokens = thisship.Tokens.CountTokensByType (typeof(Tokens.FocusToken));
			int numevadetokens = thisship.Tokens.CountTokensByType (typeof(Tokens.EvadeToken));

			if (numfocustokens > 0 && numevadetokens == 0) {
				takeFocus ();
			} else {
				if (numfocustokens == 0 && numevadetokens > 0) {
					takeEvade ();
				} else {
					if (numfocustokens > 0 && numevadetokens > 0) {
						AskWhichTokenToTake (takeFocusEventHandler, takeEvadeEventHandler);
					} else {
						SelectShipSubPhase.FinishSelection ();
					}
				}
			}
		}	

		private void takeFocus() {
			TargetShip.Tokens.RemoveToken (
				typeof(Tokens.FocusToken),
				delegate {
					HostShip.Tokens.AssignToken (
						new Tokens.FocusToken (HostShip),
						delegate {
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void takeEvade() {
			TargetShip.Tokens.RemoveToken (
				typeof(Tokens.EvadeToken),
				delegate {
					HostShip.Tokens.AssignToken (
						new Tokens.EvadeToken (HostShip),
						delegate {
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void takeFocusEventHandler(object sender, EventArgs e) {
			TargetShip.Tokens.RemoveToken (
				typeof(Tokens.FocusToken),
				delegate {
					HostShip.Tokens.AssignToken (
						new Tokens.FocusToken (HostShip),
						delegate {
							WhichTokenDecisionSubphase.ConfirmDecisionNoCallback();
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void takeEvadeEventHandler(object sender, EventArgs e) {
			TargetShip.Tokens.RemoveToken (
				typeof(Tokens.EvadeToken),
				delegate {
					HostShip.Tokens.AssignToken (
						new Tokens.EvadeToken (HostShip),
						delegate {
							WhichTokenDecisionSubphase.ConfirmDecisionNoCallback();
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void AskWhichTokenToTake(EventHandler takeFocusHandler, EventHandler takeEvadeHandler, Action callback = null)
		{
			if (callback == null)
				callback = Triggers.FinishTrigger;

			if (HostShip.Owner.Type == PlayerType.Ai) {
				takeFocus ();
			} else {

				DecisionSubPhase whichToken = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew (
					                             Name,
					                             typeof(WhichTokenDecisionSubphase),
					                             callback
				                             );

				whichToken.InfoText = "Take which type of Token?";

				whichToken.RequiredPlayer = HostShip.Owner.PlayerNo;

				whichToken.AddDecision ("Focus", takeFocusHandler);
				whichToken.AddDecision ("Evade", takeEvadeHandler);

				whichToken.ShowSkipButton = false;

				whichToken.Start ();
			}
		}

		private class WhichTokenDecisionSubphase : DecisionSubPhase { }

	}
}

