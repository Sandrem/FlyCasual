using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Tokens;
using System.Linq;
using Editions;
using SubPhases;

namespace ActionsList
{

    public class ReloadAction : GenericAction
    {

        public ReloadAction()
        {
            Name = DiceModificationName = "Reload";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

        public override void ActionTake()
        {
            RestoreCharge();
        }

        public override int GetActionPriority()
        {
            // Only perform a reload if the upgradeable ordinance has less than their maximum charges.
            // Consider reloading if we have any munitions that need it.  Increase the odds of reloading if more than one munitions card needs reloaded, as it means this ship relies heavily on munitions.
            return GetReloadableUpgrades().Count * 30;
        }

        private static List<GenericUpgrade> GetReloadableUpgrades()
        {
            return Selection.ThisShip.UpgradeBar.GetRechargableUpgrades(new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Device });
        }

        public void RestoreCharge()
        {
            List<GenericUpgrade> rechargableUpgrades = GetReloadableUpgrades();

            if (rechargableUpgrades.Count > 1)
            {
                StartDecisionSubphase();
            }
            else if (rechargableUpgrades.Count == 1)
            {
                RechargeUpgrade(rechargableUpgrades[0]);
                AssignTokenAndFinish();
            }
            else
            {
                Messages.ShowError("This ship has no upgrades that can have their charges restored");
                Phases.CurrentSubPhase.CallBack();
            }
        }

        private static void StartDecisionSubphase()
        {
            ReloadDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<ReloadDecisionSubphase>("Choose one device to reload", AssignTokenAndFinish);

            subphase.DescriptionShort = "Reload: Choose one device to regain one charge";
            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade upgrade in GetReloadableUpgrades())
            {
                subphase.AddDecision(
                    upgrade.UpgradeInfo.Name,
                    delegate { RechargeUpgradeAndFinish(upgrade); },
                    upgrade.ImageUrl,
                    upgrade.State.Charges
                );
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private static void RechargeUpgradeAndFinish(GenericUpgrade upgrade)
        {
            RechargeUpgrade(upgrade);

            DecisionSubPhase.ConfirmDecision();
        }

        private static void RechargeUpgrade(GenericUpgrade upgrade)
        {
            int count = upgrade.HostShip.GetReloadChargesCount(upgrade);
            upgrade.State.RestoreCharges(count);

            string chargesText = (count == 1) ? "1 Charge" : $"{count} Charges";
            Messages.ShowInfo($"Reload: {chargesText} of {upgrade.UpgradeInfo.Name} is restored");
        }

        protected static void AssignTokenAndFinish()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), Phases.CurrentSubPhase.CallBack);
        }

        public class ReloadDecisionSubphase : DecisionSubPhase { }

    }

}
