using Ship;
using Upgrade;
using System.Linq;
using Tokens;
using System;
using GameModes;
using Movement;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class AmilynHoldo : GenericUpgrade
    {
        public AmilynHoldo() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Amilyn Holdo",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.AmilynHoldoCrewAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/cbe5e849e9daa4f3f968b2ff6e2879b1.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class AmilynHoldoCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToSelectShip);
        }

        private void AskToSelectShip(object sender, EventArgs e)
        {
            List<GenericShip> ships = BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly);
            if (ships.Count > 0)
            {
                SelectTargetForAbility(
                    ShipIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    "Amilyn Holdo",
                    "Choose another friendly ship to transfer tokens",
                    imageSource: HostUpgrade
                );
            }
            else
            {
                //No ships in range
                Triggers.FinishTrigger();
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 1, 2) && FilterByTargetType(ship, TargetTypes.OtherFriendly);
        }

        private void ShipIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            SelectTokenToReassignToTarget();
        }

        private void SelectTokenToReassignToTarget()
        {
            SelectTokenToReassignSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectTokenToReassignSubphase>(
                "Reassign Token",
                SelectTokenToReassignFromTarget
            );

            subphase.Name = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionShort = "Select a token to transfer to the target";
            subphase.ImageSource = HostUpgrade;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (!TargetShip.Tokens.HasToken(token.GetType()) && GenericToken.SupportedTokenTypes.Contains(token.GetType()))
                {
                    subphase.AddDecision(
                        token.Name + ((token.GetType() == typeof(RedTargetLockToken)) ? " \"" + (token as RedTargetLockToken).Letter + "\"" : ""),
                        delegate { ActionsHolder.ReassignToken(token, HostShip, TargetShip, DecisionSubPhase.ConfirmDecision); }
                    );
                }
            }

            if (subphase.GetDecisions().Count > 0)
            {
                subphase.Start();
            }
            else
            {
                Messages.ShowInfoToHuman("Amilyn Holdo: No tokens to transfer to the target");
                SelectTokenToReassignFromTarget();
            }
        }

        private void SelectTokenToReassignFromTarget()
        {
            SelectTokenToReassignSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectTokenToReassignSubphase>(
                "Reassign Token",
                Triggers.FinishTrigger
            );

            subphase.Name = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionShort = "Select a token to transfer from the target";
            subphase.ImageSource = HostUpgrade;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            foreach (GenericToken token in TargetShip.Tokens.GetAllTokens())
            {
                if (!HostShip.Tokens.HasToken(token.GetType()) && GenericToken.SupportedTokenTypes.Contains(token.GetType()) )
                {
                    subphase.AddDecision(
                        token.Name + ((token.GetType() == typeof(RedTargetLockToken)) ? " \"" + (token as RedTargetLockToken).Letter + "\"" : ""),
                        delegate { ActionsHolder.ReassignToken(token, TargetShip, HostShip, DecisionSubPhase.ConfirmDecision); }
                    );
                }
            }

            if (subphase.GetDecisions().Count > 0)
            {
                subphase.Start();
            }
            else
            {
                Messages.ShowInfoToHuman("Amilyn Holdo: No tokens to transfer from the target");
                Triggers.FinishTrigger();
            }
        }

        private class SelectTokenToReassignSubphase : DecisionSubPhase { }
    }
}