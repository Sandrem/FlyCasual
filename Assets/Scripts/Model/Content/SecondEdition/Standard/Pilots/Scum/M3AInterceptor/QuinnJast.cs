using Content;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Quinn Jast",
                    "Fortune Seeker",
                    Faction.Scum,
                    3,
                    3,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.QuinnJastAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
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
            if (GetUpgradesSpentCharges().Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseQuinnJastAbility);
            }
        }

        private List<GenericUpgrade> GetUpgradesSpentCharges()
        {
            return HostShip.UpgradeBar.GetUpgradesAll()
                .Where(upgrade => upgrade.State.UsesCharges && !upgrade.UpgradeInfo.CannotBeRecharged && upgrade.State.Charges < upgrade.State.MaxCharges)
                .ToList();            
        }

        private void UseQuinnJastAbility(object sender, System.EventArgs e)
        {
            var upgrades = GetUpgradesSpentCharges();

            if (upgrades.Any()) {

                var phase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    HostName + ": Select upgrade to recover 1 charge",
                    Triggers.FinishTrigger);

                phase.DescriptionShort = HostName;
                phase.DescriptionShort = HostName + ": Gain 1 Disarm token to recover 1 charge on an upgrade";
                phase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

                upgrades.ForEach(upgrade =>
                {
                    phase.AddDecision(upgrade.UpgradeInfo.Name, delegate {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        upgrade.State.RestoreCharge();
                        HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
                    }, upgrade.ImageUrl);
                });

                phase.DefaultDecisionName = phase.GetDecisions().First().Name;
                phase.ShowSkipButton = true;

                phase.Start();
            }
        }
    }
}