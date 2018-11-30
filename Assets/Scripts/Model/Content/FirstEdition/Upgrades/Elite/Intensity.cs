using Upgrade;
using Ship;
using ActionsList;
using System;
using SubPhases;
using System.Collections.Generic;
using UnityEngine;
using Tokens;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Intensity : GenericDualUpgrade
    {
        public Intensity() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Intensity",
                UpgradeType.Elite,
                cost: 2,
                restriction: new BaseSizeRestriction(BaseSize.Small),
                abilityType: typeof(Abilities.FirstEdition.IntensityAbility)
            );

            AnotherSide = typeof(IntensityExhausted);

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(49, 0));
        }
    }

    public class IntensityExhausted : GenericDualUpgrade
    {
        public IntensityExhausted() : base()
        {
            IsHidden = true; // Hidden in Squad Builder only

            UpgradeInfo = new UpgradeCardInfo(
                "Intensity (Exhausted)",
                UpgradeType.Elite,
                cost: 2,
                restriction: new BaseSizeRestriction(BaseSize.Small),
                abilityType: typeof(Abilities.FirstEdition.IntensityExhaustedAbility)
            );

            AnotherSide = typeof(Intensity);

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(39, 0));
        }
    }
}

namespace Abilities.FirstEdition
{
    public class IntensityAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction actionTaken)
        {
            if (actionTaken is BoostAction || actionTaken is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToAssignToken);
            }
        }

        private void AskToAssignToken(object sender, System.EventArgs e)
        {
            //Skip if side is wrong
            if (!ConditionsStillCorrect())
            {
                Triggers.FinishTrigger();
                return;
            }

            SelectTokenDecisionSubphase decision = (SelectTokenDecisionSubphase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(SelectTokenDecisionSubphase),
                Triggers.FinishTrigger
            );

            decision.InfoText = "Select token to assign and flip Intensity to Exhausted side";

            decision.AddDecision("Focus Token", delegate { AssignToken(typeof(FocusToken)); });
            decision.AddDecision("Evade Token", delegate { AssignToken(typeof(EvadeToken)); });

            decision.DefaultDecisionName = GetBestToken();

            decision.RequiredPlayer = HostShip.Owner.PlayerNo;

            decision.ShowSkipButton = true;

            decision.Start();
        }

        private bool ConditionsStillCorrect()
        {
            if (!HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n is UpgradesList.FirstEdition.Intensity)) return false;

            return true;
        }

        private string GetBestToken()
        {
            string bestToken = "";

            if (HostShip.Tokens.HasToken(typeof(EvadeToken)))
            {
                // Focus token if has evade only or evade + focus
                bestToken = "Focus Token";
            }
            else if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                // Evade token if has focus only
                bestToken = "Evade Token";
            }
            else
            {
                // Focus token if has no tokens at all
                bestToken = "Focus Token";
            }

            return bestToken;
        }

        private void AssignToken(Type tokenType)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();

            HostShip.Tokens.AssignToken(tokenType, DecisionSubPhase.ConfirmDecision);
        }

        private class SelectTokenDecisionSubphase : DecisionSubPhase { }
    }

    public class IntensityExhaustedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)) || HostShip.Tokens.HasToken(typeof(EvadeToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, AskToSpendToken);
            }
        }

        private void AskToSpendToken(object sender, System.EventArgs e)
        {
            //Skip if side is wrong or tokens are not present
            if (!ConditionsStillCorrect())
            {
                Triggers.FinishTrigger();
                return;
            }

            SelectTokenDecisionSubphase decision = (SelectTokenDecisionSubphase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(SelectTokenDecisionSubphase),
                Triggers.FinishTrigger
            );

            decision.InfoText = "Select token to spend to flip Intensity";

            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                decision.AddDecision("Focus Token", delegate { SpendTokenToRestoreUpgrade(typeof(FocusToken)); });
            }

            if (HostShip.Tokens.HasToken(typeof(EvadeToken)))
            {
                decision.AddDecision("Evade Token", delegate { SpendTokenToRestoreUpgrade(typeof(EvadeToken)); });
            }

            decision.DefaultDecisionName = GetBestToken();

            decision.RequiredPlayer = HostShip.Owner.PlayerNo;

            decision.ShowSkipButton = true;

            decision.Start();
        }

        private bool ConditionsStillCorrect()
        {
            if (!HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n is UpgradesList.FirstEdition.IntensityExhausted)) return false;

            if (!HostShip.Tokens.HasToken(typeof(FocusToken)) && !HostShip.Tokens.HasToken(typeof(EvadeToken))) return false;

            return true;
        }

        private string GetBestToken()
        {
            string bestToken = "Focus Token";

            if (HostShip.Tokens.HasToken(typeof(EvadeToken)))
            {
                bestToken = "Evade Token";
            }

            return bestToken;
        }

        private void SpendTokenToRestoreUpgrade(Type tokenType)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();

            HostShip.Tokens.SpendToken(tokenType, DecisionSubPhase.ConfirmDecision);
        }

        private class SelectTokenDecisionSubphase : DecisionSubPhase { }
    }
}