using Ship;
using Upgrade;
using System;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Rey : GenericUpgrade
    {
        public Rey() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rey",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.ReyCrewAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ReyCrewAbility : GenericAbility
    {
        private int focusTokensStored;

        public int FocusTokensStored
        {
            get { return focusTokensStored; }
            set
            {
                focusTokensStored = value;
                UpdateNameOfUpgrade(value);
            }
        }

        private void UpdateNameOfUpgrade(int value)
        {
            string postfix = (focusTokensStored == 0) ? "" : " (" + focusTokensStored + ")";
            HostUpgrade.NamePostfix = postfix;
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAssignTokenAbility;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterStoreTokenAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAssignTokenAbility;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterStoreTokenAbility;
        }

        private void RegisterStoreTokenAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, CheckStoreTokenAbility);
        }

        private void CheckStoreTokenAbility(object sender, System.EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(
                        HostUpgrade.UpgradeInfo.Name,
                        AlwaysUseByDefault,
                        StoreToken,
                        descriptionLong: "Do you want to place 1 of your ship's Focus Tokens on this card?",
                        imageHolder: HostUpgrade,
                        showAlwaysUseOption: true
                    );
                }
                else
                {
                    DoStoreToken(Triggers.FinishTrigger);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void StoreToken(object sender, System.EventArgs e)
        {
            DoStoreToken(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void DoStoreToken(Action callback)
        {
            FocusTokensStored++;
            HostShip.Tokens.RemoveToken(
                typeof(FocusToken),
                callback
            );
        }

        private void RegisterAssignTokenAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, CheckAssignTokenAbility);
        }

        private void CheckAssignTokenAbility(object sender, System.EventArgs e)
        {
            if (FocusTokensStored > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    AssignStoredToken,
                    descriptionLong: "Do you want to assign 1 of stored Focus tokens to your ship?",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AssignStoredToken(object sender, System.EventArgs e)
        {
            FocusTokensStored--;
            HostShip.Tokens.AssignToken(typeof(FocusToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

    }
}