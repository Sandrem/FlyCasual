﻿using ActionsList;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AdvancedSlam : GenericUpgrade
    {
        public AdvancedSlam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Slam",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.AdvancedSlamAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AdvancedSlamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckSlamAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckSlamAction;
        }

        protected virtual void CheckSlamAction(GenericAction action)
        {
            if (action is SlamAction)
            {
                if (HostShip.IsBumped)
                {
                    Messages.ShowErrorToHuman("SLAM maneuver overlapped another ship, Advanced SLAM action is skipped");
                }
                else if (HostShip.IsHitObstacles)
                {
                    Messages.ShowErrorToHuman("SLAM maneuver overlapped an obstacle, Advanced SLAM action is skipped");
                }
                else
                {
                    RegisterTrigger();
                }
            }
        }

        protected void RegisterTrigger()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Advanced SLAM",
                TriggerType = TriggerTypes.OnActionIsPerformed,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = PerfromFreeActionFromUpgradeBar
            });
        }

        protected virtual void PerfromFreeActionFromUpgradeBar(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();
            List<GenericAction> actionBarActions = actions.Where(n => n.IsInActionBar).ToList();

            Selection.ThisShip.AskPerformFreeAction(
                actionBarActions,
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                "After performing a SLAM action, if you did not overlap an obstacle or another ship, you may perform a free action on your action bar",
                HostUpgrade
            );
        }
    }
}