using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class CorusKapellim : BTANR2YWing
        {
            public CorusKapellim() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Corus Kapellim",
                    "\"Gentleman Flyer\"",
                    Faction.Resistance,
                    1,
                    4,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CorusKapellimAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://i.imgur.com/0uhUP03.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CorusKapellimAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, CheckAbility);
        }

        private void CheckAbility(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Selection.ChangeActiveShip(HostShip);
                Messages.ShowInfoToHuman(HostShip.PilotInfo.PilotName + ": Select a ship to remove 1 green token from");

                SelectTargetForAbility
                (
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a ship to remove 1 green token from it and assign this token to yourself",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 1)
                && ship.Tokens.HasGreenTokens;
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);
            result += ship.Tokens.CountTokensByColor(TokenColors.Green) * 100;
            if (Tools.IsSameTeam(HostShip, ship)) result -= 1000;

            return result;
        }

        private void SelectAbilityTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            GenericShip sufferedShip = TargetShip;
            List<GenericToken> sufferedTokens = TargetShip.Tokens.GetTokensByColor(TokenColors.Green);

            if (sufferedTokens.Count == 1)
            {
                TakeToken(sufferedTokens.First());
            }
            else
            {
                List<Type> tokenTypes = new List<Type>();
                foreach (GenericToken token in sufferedTokens)
                {
                    if (!tokenTypes.Contains(token.GetType())) tokenTypes.Add(token.GetType());
                }

                if (tokenTypes.Count == 1)
                {
                    TakeToken(sufferedTokens.First());
                }
                else
                {
                    AskWhichTokenToTake(sufferedTokens);
                }
            }
        }

        private void TakeToken(GenericToken token)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {token.Name} is stolen from {TargetShip.PilotInfo.PilotName}");

            TargetShip.Tokens.RemoveToken(
                token,
                delegate {
                    HostShip.Tokens.AssignToken(
                        token.GetType(),
                        Triggers.FinishTrigger
                    );
                }
            );
        }

        private void AskWhichTokenToTake(List<GenericToken> sufferedTokens)
        {
            DecisionSubPhase subphase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(WhichTokenDecisionSubphase),
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Take which type of Token?";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;

            foreach (GenericToken token in sufferedTokens)
            {
                if (!subphase.GetDecisions().Any(n => n.Name == token.Name))
                {
                    subphase.AddDecision
                    (
                        token.Name,
                        delegate {
                            DecisionSubPhase.ConfirmDecisionNoCallback();
                            TakeToken(token); 
                        }
                    );
                }
            }

            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private class WhichTokenDecisionSubphase : DecisionSubPhase { }

    }
}
