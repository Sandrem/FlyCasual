using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class QuinnJast : M3AInterceptor
        {
            public QuinnJast() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Quinn Jast",
                    3,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.QuinnJastAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 186
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class QuinnJastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            if (GetHardpointWithSpentCharges() != null)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseQuinnJastAbility);
            }
        }

        private void AskUseQuinnJastAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseQuinnJastAbility);
        }

        private GenericUpgrade GetHardpointWithSpentCharges()
        {
            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesAll())
            {
                if
                (
                    upgrade.HasType(UpgradeType.Missile) ||
                    upgrade.HasType(UpgradeType.Cannon) ||
                    upgrade.HasType(UpgradeType.Torpedo)
                )
                {
                    if (upgrade.State.UsesCharges && upgrade.State.Charges < upgrade.State.MaxCharges)
                    {
                        return upgrade;
                    }
                }
            }

            return null;
        }

        private void UseQuinnJastAbility(object sender, System.EventArgs e)
        {
            GenericUpgrade spentUpgrade = GetHardpointWithSpentCharges();

            if (spentUpgrade != null)
            {
                HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () => {
                    spentUpgrade.State.RestoreCharge();
                    DecisionSubPhase.ConfirmDecision();
                });
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}