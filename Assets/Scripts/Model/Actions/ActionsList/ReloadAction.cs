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

        public ReloadAction() {
            Name = EffectName = "Reload";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

        public override void ActionTake()
        {
            foreach (var upgrade in Selection.ThisShip.UpgradeBar.GetInstalledUpgrades())
            {
                if (upgrade.hasType(UpgradeType.Missile) || upgrade.hasType(UpgradeType.Torpedo))
                {
                    if (upgrade.isDiscarded) upgrade.FlipFaceup();
                }
            }

            Selection.ThisShip.AssignToken(new WeaponsDisabledToken(), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;

            int discardedOrdnance = Selection.ThisShip.UpgradeBar.GetInstalledUpgrades().Count(n => (n.hasType(UpgradeType.Missile) || n.hasType(UpgradeType.Torpedo)) && n.isDiscarded);
            result = discardedOrdnance * 30;

            return result;
        }

    }

}
