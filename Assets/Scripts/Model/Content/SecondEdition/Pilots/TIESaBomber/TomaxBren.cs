using ActionsList;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class TomaxBren : TIESaBomber, TIE
        {
            public TomaxBren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tomax Bren",
                    5,
                    34,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.TomaxBrenAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 107;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TomaxBrenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is ReloadAction && HasEliteUpgradesToRecharge())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, ShowEliteUpgradeToRecharge);
            }
        }

        private bool HasEliteUpgradesToRecharge()
        {
            return GetEliteUpgradesToRecharge().Count > 0;
        }

        private List<GenericUpgrade> GetEliteUpgradesToRecharge()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n.UpgradeInfo.HasType(UpgradeType.Elite) && (n.Charges < n.MaxCharges)).ToList();
        }

        private void ShowEliteUpgradeToRecharge(object sender, System.EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<TomaxBrenDecisionSubphase>(
                "Tomax Bren's ability",
                Triggers.FinishTrigger
            );
            phase.InfoText = "You may recover a charge:";
            phase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            phase.ShowSkipButton = true;

            List<GenericUpgrade> AvailableUpgrades = GetEliteUpgradesToRecharge();

            foreach (var upgrade in AvailableUpgrades)
            {
                phase.AddDecision(
                    upgrade.Name,
                    delegate { RestoreCharge(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.Charges
                );
            }

            phase.DefaultDecisionName = AvailableUpgrades[0].Name;

            phase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            phase.Start();
        }

        protected void RestoreCharge(GenericUpgrade upgrade)
        {
            upgrade.RestoreCharge();
            DecisionSubPhase.ConfirmDecision();
        }

        public class TomaxBrenDecisionSubphase : DecisionSubPhase { }
    }
}