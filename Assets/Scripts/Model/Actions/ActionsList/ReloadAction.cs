using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Tokens;
using System.Linq;

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
            FlipFaceupRecursive();
        }

        private void FlipFaceupRecursive()
        {
            GenericUpgrade discardedUpgrade = null;

            List<GenericUpgrade> discardedUpgrades = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded();
            if (discardedUpgrades.Count != 0) discardedUpgrade = discardedUpgrades.FirstOrDefault(n => n.hasType(UpgradeType.Missile) || n.hasType(UpgradeType.Torpedo));

            if (discardedUpgrade != null)
            {
                discardedUpgrade.FlipFaceup(FlipFaceupRecursive);
            }
            else
            {
                Selection.ThisShip.Tokens.AssignToken(new WeaponsDisabledToken(Selection.ThisShip), Phases.CurrentSubPhase.CallBack);
            }
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int discardedOrdnance = Selection.ThisShip.UpgradeBar.GetUpgradesOnlyDiscarded().Count(n => n.hasType(UpgradeType.Missile) || n.hasType(UpgradeType.Torpedo));
            result = discardedOrdnance * 30;

            return result;
        }

    }

}
