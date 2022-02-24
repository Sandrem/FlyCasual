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
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TomaxBrenAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 107
                );

                ModelInfo.SkinName = "White Death";
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
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n.UpgradeInfo.HasType(UpgradeType.Talent) && (n.State.Charges < n.State.MaxCharges)).ToList();
        }

        private void ShowEliteUpgradeToRecharge(object sender, System.EventArgs e)
        {
            var phase = Phases.StartTemporarySubPhaseNew<TomaxBrenDecisionSubphase>(
                HostShip.PilotInfo.PilotName,
                Triggers.FinishTrigger
            );

            phase.DescriptionShort = HostShip.PilotInfo.PilotName;
            phase.DescriptionLong = "You may recover a charge:";
            phase.ImageSource = HostShip;

            phase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            phase.ShowSkipButton = true;

            List<GenericUpgrade> AvailableUpgrades = GetEliteUpgradesToRecharge();

            foreach (var upgrade in AvailableUpgrades)
            {
                phase.AddDecision(
                    upgrade.UpgradeInfo.Name,
                    delegate { RestoreCharge(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.State.Charges
                );
            }

            phase.DefaultDecisionName = AvailableUpgrades[0].UpgradeInfo.Name;

            phase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            phase.Start();
        }

        protected void RestoreCharge(GenericUpgrade upgrade)
        {
            upgrade.State.RestoreCharge();
            DecisionSubPhase.ConfirmDecision();
        }

        public class TomaxBrenDecisionSubphase : DecisionSubPhase { }
    }
}