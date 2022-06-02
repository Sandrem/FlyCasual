using System;
using System.Collections.Generic;
using System.Linq;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEBaInterceptor
    {
        public class Holo : TIEBaInterceptor
        {
            public Holo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Holo\"",
                    "Trick of the Light",
                    Faction.FirstOrder,
                    5,
                    5,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HoloAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/ee53482be8e59ff44f272e76c4e8123d.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // At the start of the Engagement Phase, you must transfer 1 of your tokens to another friendly ship at range 0-2.

    public class HoloAbility : GenericAbility
    {
        private GenericToken SelectedToken;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            // Blue target locks don't count - they are only as FE legacy
            int tokensCount = HostShip.Tokens.GetAllTokens().Where(n => n.GetType() != typeof(BlueTargetLockToken)).Count();
            if (tokensCount > 0)
            {
                int filteredShipsCount = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count(n => n.ShipId != HostShip.ShipId);

                if (filteredShipsCount > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectToken);
                }
            }
        }

        private void AskToSelectToken(object sender, EventArgs e)
        {
            var ownTokens = HostShip.Tokens.GetAllTokens().Where(n => n.GetType() != typeof(BlueTargetLockToken))
                .Distinct(new TokenComparer())
                .ToList();

            if (ownTokens.Any())
            {
                TokenSelectionSubphase subphase = (TokenSelectionSubphase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(TokenSelectionSubphase),
                    Triggers.FinishTrigger
                );

                subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
                subphase.DescriptionLong = "Select token to transfer";
                subphase.ImageSource = HostShip;

                ownTokens.ForEach(token =>
                {
                    subphase.AddDecision(
                        (token is RedTargetLockToken) ? token.Name + " \"" + (token as RedTargetLockToken).Letter + "\"" : token.Name,
                        delegate {
                            SelectedToken = token;
                            DecisionSubPhase.ConfirmDecisionNoCallback();
                            AskToSelectShipToTransferToken();
                        }
                    );
                });

                subphase.RequiredPlayer = HostShip.Owner.PlayerNo;
                subphase.ShowSkipButton = false;

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

                subphase.Start();
            }
            else
            {
                Messages.ShowError("No tokens to transfer");
                Triggers.FinishTrigger();
            }
        }

        private void AskToSelectShipToTransferToken()
        {
            SelectTargetForAbility(
                AskWhatTokenToTransfer,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Select another friendly ship to transfer your token",
                imageSource: HostShip
            );
        }

        private void AskWhatTokenToTransfer()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.ReassignToken(SelectedToken, HostShip, TargetShip, Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 2) && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.ShipId != HostShip.ShipId;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return -ship.PilotInfo.Cost;
        }

        private class TokenSelectionSubphase : DecisionSubPhase { }
    }
}