using Actions;
using ActionsList;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AdvancedSlam : GenericUpgrade
    {
        public AdvancedSlam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Slam",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.AdvancedSlamAbility),
                restriction: new ActionBarRestriction(typeof(SlamAction)),
                seImageNumber: 69
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedSlamAbility : Abilities.FirstEdition.AdvancedSlamAbility
    {
        protected override void PerfromFreeActionFromUpgradeBar(object sender, EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();
            List<GenericAction> whiteActionBarActionsAsRed = actions
                .Where(n => n.IsInActionBar && !n.IsRed)
                .Select(n => n.AsRedAction)
                .ToList();

            Selection.ThisShip.AskPerformFreeAction(whiteActionBarActionsAsRed, Triggers.FinishTrigger);
        }
    }
}