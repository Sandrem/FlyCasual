using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;
using BoardTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class PZ4CO : GenericUpgrade
    {
        public PZ4CO() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "PZ-4CO",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                addAction: new ActionInfo(typeof(CalculateAction)),
                abilityType: typeof(Abilities.SecondEdition.PZ4COAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(269, 7),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class PZ4COAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers += RegisterOwnTrigger;
        }
                
        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseEnd, AskToSelectShipForOwnAbility);
        }

        private void AskToSelectShipForOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                AskToTransferToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "You may choose a friendly ship at range 1-2 to transfer a token",
                imageSource: HostUpgrade
            );
        }

        private void AskToTransferToken()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            bool canTransferCalculate = HostShip.Tokens.HasToken(typeof(CalculateToken));

            bool canTransferFocus = HostShip.Tokens.HasToken(typeof(FocusToken)) && HostShip.RevealedManeuver != null && HostShip.RevealedManeuver.ColorComplexity == Movement.MovementComplexity.Easy;

            if (canTransferCalculate && !canTransferFocus)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    TransferCalculateToken,
                    descriptionLong: "Do you want to transfer 1 Calculate token to that ship?",
                    imageHolder: HostUpgrade,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else if (!canTransferCalculate && canTransferFocus)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    TransferFocusToken,
                    descriptionLong: "Do you want to transfer 1 Focus token to that ship?",
                    imageHolder: HostUpgrade,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else if (canTransferCalculate && canTransferFocus)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    TransferFocusToken,
                    dontUseAbility: TransferCalculateToken,
                    descriptionLong: "Do you want to transfer 1 Focus token to that ship instead of 1 Calculate token?",
                    imageHolder: HostUpgrade,
                    requiredPlayer: HostShip.Owner.PlayerNo,
                    showSkipButton: false
                );
            }
            else
            {
                Messages.ShowErrorToHuman(HostUpgrade.UpgradeInfo.Name + ": You don't have any required token to transfer");
                Triggers.FinishTrigger();
            }
        }

        private void TransferCalculateToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(
                typeof(CalculateToken),
                delegate{
                    TargetShip.Tokens.AssignToken(
                        typeof(CalculateToken),
                        delegate {
                            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Calculate Token is reassigned to " + TargetShip.PilotInfo.PilotName);
                            Triggers.FinishTrigger();
                        }
                    );
                }
            );
        }

        private void TransferFocusToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.RemoveToken(
                typeof(FocusToken),
                delegate {
                    TargetShip.Tokens.AssignToken(
                        typeof(FocusToken),
                        delegate {
                            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Focus Token is reassigned to " + TargetShip.PilotInfo.PilotName);
                            Triggers.FinishTrigger();
                        }
                    );
                }
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range >= 1 && distInfo.Range <= 2;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return -ship.Tokens.GetAllTokens().Count(n => n.TokenColor == TokenColors.Green) * 100 + ship.PilotInfo.Cost;
        }
    }
}