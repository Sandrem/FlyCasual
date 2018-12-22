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
            Edition.Current.ReloadAction();
        }

        public static void FlipFaceupRecursive()
        {
            GenericUpgrade discardedUpgrade = null;

            List<GenericUpgrade> discardedUpgrades = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded();
            if (discardedUpgrades.Count != 0) discardedUpgrade = discardedUpgrades.FirstOrDefault(n => n.HasType(UpgradeType.Missile) || n.HasType(UpgradeType.Torpedo));

            if (discardedUpgrade != null)
            {
                discardedUpgrade.FlipFaceup(FlipFaceupRecursive);
            }
            else
            {
                AssignTokenAndFinish();
            }
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int discardedOrdnance = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded().Count(n => n.HasType(UpgradeType.Missile) || n.HasType(UpgradeType.Torpedo));
            result = discardedOrdnance * 30;

            return result;
        }

        private static List<GenericUpgrade> GetReloadableUpgrades()
        {
            return Selection.ThisShip.UpgradeBar.GetRechargableUpgrades(new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Bomb });
        }

        public static void RestoreOneCharge()
        {
            List<GenericUpgrade> rechargableUpgrades = GetReloadableUpgrades();

            if (rechargableUpgrades.Count > 1)
            {
                StartDecisionSubphase();
            }
            else if (rechargableUpgrades.Count == 1)
            {
                rechargableUpgrades[0].State.RestoreCharge();
                Messages.ShowInfo("Reload: One charge of \"" + rechargableUpgrades[0].UpgradeInfo.Name + "\" is restored");
                AssignTokenAndFinish();
            }
            else
            {
                Messages.ShowError("No upgrades to restore charge");
                Phases.CurrentSubPhase.CallBack();
            }
        }

        private static void StartDecisionSubphase()
        {
            ReloadDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<ReloadDecisionSubphase>("Choose device to reload", AssignTokenAndFinish);

            subphase.InfoText = "Choose device to restore one charge";
            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade upgrade in GetReloadableUpgrades())
            {
                subphase.AddDecision(upgrade.UpgradeInfo.Name, delegate { RechargeUpgrade(upgrade); }, upgrade.ImageUrl, upgrade.State.Charges);
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private static void AssignTokenAndFinish()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), Phases.CurrentSubPhase.CallBack);
        }

        private static void RechargeUpgrade(GenericUpgrade upgrage)
        {
            upgrage.State.RestoreCharge();
            Messages.ShowInfo("Reload: One charge of \"" + upgrage.UpgradeInfo.Name + "\" is restored");

            DecisionSubPhase.ConfirmDecision();
        }

        public class ReloadDecisionSubphase : DecisionSubPhase { }

    }

}
