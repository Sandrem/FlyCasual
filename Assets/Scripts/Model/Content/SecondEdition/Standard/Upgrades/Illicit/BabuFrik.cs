using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BabuFrik : GenericUpgrade
    {
        public BabuFrik() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Babu Frik",
                UpgradeType.Illicit,
                cost: 5,
                isLimited: true,
                charges: 3,
                restriction: new FactionRestriction(Faction.Resistance, Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.BabuFrikAbility)
            );

            ImageUrl = "https://i.imgur.com/pSLHvH1.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BabuFrikAbility : GenericAbility
    {
        List<GenericToken> PlacedTokens = new List<GenericToken>();
        GenericToken CurrentToken;

        public override void ActivateAbility()
        {
            HostShip.BeforeTokenIsAssigned += CheckAbilityOnAssigned;
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += CheckRemoval;
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped += CheckSystemPhaseSkip;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeTokenIsAssigned -= CheckAbilityOnAssigned;
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= CheckRemoval;
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped -= CheckSystemPhaseSkip;
        }

        private void CheckAbilityOnAssigned(GenericShip ship, GenericToken token)
        {
            if (IsOrangeOrRedNonLock(token) && HostUpgrade.State.Charges > 0)
            {
                CurrentToken = token;
                RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, AskToPlaceOnCard);
            }
        }

        private bool IsOrangeOrRedNonLock(GenericToken token)
        {
            if (token is null) return false;
            if (token.TokenColor == TokenColors.Orange) return true;
            if (token.TokenColor == TokenColors.Red && !(token is RedTargetLockToken)) return true;

            return false;
        }

        private void AskToPlaceOnCard(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                PlaceTokenOnCardInstead,
                descriptionLong: $"Do you want to place {CurrentToken.Name} on this card instead?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void PlaceTokenOnCardInstead(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            PlacedTokens.Add(CurrentToken);
            HostShip.Tokens.TokenToAssign = null;

            UpdateUpgradeText();
            Messages.ShowInfo($"{CurrentToken.Name} is placed on {HostUpgrade.UpgradeInfo.Name} instead");

            DecisionSubPhase.ConfirmDecision();
        }

        private void UpdateUpgradeText()
        {
            string postfix = GetTokenNames();
            HostUpgrade.NamePostfix = postfix;
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        private string GetTokenNames()
        {
            string tokenNames = "";

            foreach (GenericToken token in PlacedTokens)
            {
                if (tokenNames != "") tokenNames += ", ";
                tokenNames += token.Name.Replace(" Token", "");
            }

            if (tokenNames != "") tokenNames = " (" + tokenNames + ")";

            return tokenNames;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (PlacedTokens.Count > 0) flag = true;
        }

        private void CheckRemoval(GenericShip ship)
        {
            if (PlacedTokens.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToKeepTokens);
            }
        }

        private void AskToKeepTokens(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0) KeepTokens(); else ReleaseTokens();
        }

        private void KeepTokens()
        {
            HostUpgrade.State.SpendCharge();
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: Charge is spend to keep tokens");
            Triggers.FinishTrigger();
        }

        private void ReleaseTokens()
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: Tokens are released");
            ReleaseTokensRecursive(Triggers.FinishTrigger);
        }

        private void ReleaseTokensRecursive(Action callBack)
        {
            if (PlacedTokens.Count > 0)
            {
                GenericToken tokenToPlace = PlacedTokens.First();
                PlacedTokens.Remove(tokenToPlace);

                HostShip.Tokens.AssignToken(tokenToPlace, delegate { ReleaseTokensRecursive(callBack); });
            }
            else
            {
                UpdateUpgradeText();
                callBack();
            }
        }

        private void CheckSystemPhaseSkip(ref bool canBeSkipped)
        {
            if (PlacedTokens.Count > 0) canBeSkipped = false;
        }
    }
}

