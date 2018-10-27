using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;
using Players;
using System.Linq;
using Tokens;
using RuleSets;
using BoardTools;
using Arcs;

namespace Ship
{
	namespace HWK290
	{
		public class PalobGodalhi : HWK290, ISecondEditionPilot
		{
			public PalobGodalhi() : base()
			{
				PilotName = "Palob Godalhi";
				PilotSkill = 5;
				Cost = 20;

				IsUnique = true;

				PrintedUpgradeIcons.Add(UpgradeType.Elite);
				PrintedUpgradeIcons.Add(UpgradeType.Illicit);

				faction = Faction.Scum;

				PilotAbilities.Add(new Abilities.PalobGodalhi());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 38;

                PilotAbilities.RemoveAll(ability => ability is Abilities.PalobGodalhi);
                PilotAbilities.Add(new Abilities.SecondEdition.PalobGodalhiSE());

                SEImageNumber = 175;
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
			Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
		}

		public override void DeactivateAbility()
		{
			Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
		}

		private void RegisterAbility()
		{
			RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
		}

		private void Ability(object sender, EventArgs e)
		{
			if (TargetsForAbilityExist (FilterAbilityTarget)) {
				Messages.ShowInfoToHuman ("Palob Godalhi: Select a ship to remove Focus/Evade token from");

                SelectTargetForAbility(
					SelectAbilityTarget,
					FilterAbilityTarget,
					GetAiAbilityPriority,
					HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
                    "Choose a ship to remove 1 focus or evade token from it and assign this token to yourself.",
                    HostShip
                );
			} else {
					Triggers.FinishTrigger();
			}
		}

		protected virtual bool FilterAbilityTarget(GenericShip ship)
		{
			return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 2) && FilterTargetWithTokens(ship);
		}

		protected bool FilterTargetWithTokens(GenericShip ship)
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
				TakeFocus ();
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

		private void TakeFocus() {
			TargetShip.Tokens.RemoveToken (
				typeof(FocusToken),
				delegate {
					HostShip.Tokens.AssignToken (
                        typeof(FocusToken),
						delegate {
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void takeEvade() {
			TargetShip.Tokens.RemoveToken (
				typeof(EvadeToken),
				delegate {
					HostShip.Tokens.AssignToken (
                        typeof(EvadeToken),
						delegate {
							SelectShipSubPhase.FinishSelection();
						}
					);
				}
			);
		}

		private void takeFocusEventHandler(object sender, EventArgs e) {
			TargetShip.Tokens.RemoveToken (
				typeof(FocusToken),
				delegate {
					HostShip.Tokens.AssignToken (
                        typeof(FocusToken),
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
				typeof(EvadeToken),
				delegate {
					HostShip.Tokens.AssignToken (
                        typeof(EvadeToken),
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
				TakeFocus ();
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

namespace Abilities.SecondEdition
{
    public class PalobGodalhiSE : PalobGodalhi
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return 
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) &&
                FilterTargetsByRange(ship, 0, 2) &&
                Board.IsShipInArc(HostShip, ship) &&
                FilterTargetWithTokens(ship);
        }
    }
}