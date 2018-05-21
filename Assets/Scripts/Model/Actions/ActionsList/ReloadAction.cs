using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Tokens;
using System.Linq;
using RuleSets;
using SubPhases;

namespace ActionsList
{

    public class ReloadAction : GenericAction
    {

        public ReloadAction()
        {
            Name = EffectName = "Reload";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

        public override void ActionTake()
        {
            RuleSet.Instance.ReloadAction();
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

        public static void RestoreOneCharge()
        {
            List<GenericUpgrade> rechargableUpgrades = Selection.ThisShip.UpgradeBar.GetRechargableUpgrades();

            if (rechargableUpgrades.Count > 1)
            {
                StartDecisionSubphase();
            }
            else if (rechargableUpgrades.Count == 1)
            {
                rechargableUpgrades[0].RestoreCharge();
                Messages.ShowInfo("Reload: One charge of \"" + rechargableUpgrades[0].NameOriginal + "\" is restored");
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
            subphase.DecisionViewType = DecisionViewTypes.ImageButtons;

            foreach (GenericUpgrade upgrade in Selection.ThisShip.UpgradeBar.GetRechargableUpgrades())
            {
                subphase.AddDecision(upgrade.Name, delegate { RechargeUpgrade(upgrade); }, upgrade.ImageUrl, upgrade.Charges);
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private static void AssignTokenAndFinish()
        {
            Selection.ThisShip.Tokens.AssignToken(
                new WeaponsDisabledToken(Selection.ThisShip),
                Phases.CurrentSubPhase.CallBack
            );
        }

        private static void RechargeUpgrade(GenericUpgrade upgrage)
        {
            upgrage.RestoreCharge();
            Messages.ShowInfo("Reload: One charge of \"" + upgrage.NameOriginal + "\" is restored");

            DecisionSubPhase.ConfirmDecision();
        }

        public class ReloadDecisionSubphase : DecisionSubPhase { }

    }

}
