using Upgrade;
using Ship;
using GameModes;
using Abilities;
using Tokens;
using System;

namespace UpgradesList
{
    public class Rey : GenericUpgrade
    {
        public Rey() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Rey";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new ReyCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

    }
}

namespace Abilities
{
    public class ReyCrewAbility : GenericAbility
    {
        private string originalUpgradeName = "Rey";

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
            HostUpgrade.Name = originalUpgradeName + postfix;
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);
        }

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterAssignTokenAbility;
            Phases.OnEndPhaseStart_Triggers += RegisterStoreTokenAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterAssignTokenAbility;
            Phases.OnEndPhaseStart_Triggers -= RegisterStoreTokenAbility;
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
                    AskToUseAbility(AlwaysUseByDefault, StoreToken, null, null, true);
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
                AskToUseAbility(AlwaysUseByDefault, AssignStoredToken);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AssignStoredToken(object sender, System.EventArgs e)
        {
            FocusTokensStored--;
            HostShip.Tokens.AssignToken(new FocusToken(HostShip), SubPhases.DecisionSubPhase.ConfirmDecision);
        }

    }
}
