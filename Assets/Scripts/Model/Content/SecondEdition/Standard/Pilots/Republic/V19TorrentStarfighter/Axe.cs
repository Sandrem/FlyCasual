using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Axe : V19TorrentStarfighter
    {
        public Axe()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Axe\"",
                "Blue Two",
                Faction.Republic,
                3,
                3,
                8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.AxeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                }
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/2c/ee/2ceea646-b5bd-42ce-aeb1-7f38dc88e045/swz32_axe.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you defend or perform an attack, you may choose a friendly ship at range 1-2 in your left or right arc. 
    //If you do, transfer 1 green token to that ship.
    public class AxeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostShip.Tokens.HasGreenTokens)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAbility);
            }
        }

        private void AskAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TransferToken,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostName,
                "You may transfer 1 green token to a ship in your left or right arc",
                HostShip
            );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            var result = 0;

            if (!ship.Tokens.HasGreenTokens) result += 100;

            result += ship.PilotInfo.Cost;

            return result;
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly })
                && FilterTargetsByRange(ship, 1, 2)
                && (HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Left) || HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Right));
        }

        private void TransferToken()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            var greenTokens = HostShip.Tokens.GetAllTokens()
                .Where(token => token.TokenColor == TokenColors.Green)
                .Distinct(new TokenComparer())
                .ToList();

            if (greenTokens.Any())
            {
                DecisionSubPhase phase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(DecisionSubPhase),
                    Triggers.FinishTrigger
                );

                phase.DescriptionShort = "Select token to transfer";
                phase.RequiredPlayer = HostShip.Owner.PlayerNo;
                phase.ShowSkipButton = true;

                greenTokens.ForEach(token =>
                {
                    phase.AddDecision(token.Name, delegate { HostShip.Tokens.TransferToken(token.GetType(), TargetShip, DecisionSubPhase.ConfirmDecision); });
                });

                phase.DefaultDecisionName = phase.GetDecisions().First().Name;

                phase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
