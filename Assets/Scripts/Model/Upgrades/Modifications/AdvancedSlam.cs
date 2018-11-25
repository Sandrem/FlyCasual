using Ship;
using Upgrade;
using ActionsList;
using Abilities;
using System.Collections.Generic;
using System.Linq;
using RuleSets;
using System;

namespace UpgradesList
{
    public class AdvancedSlam : GenericUpgrade, ISecondEditionUpgrade
    {
        public AdvancedSlam() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Advanced SLAM";
            Cost = 2;

            UpgradeAbilities.Add(new AdvancedSlamAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;
            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new Abilities.SecondEdition.AdvancedSlamAbility());
            SEImageNumber = 69;            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (RuleSet.Instance is SecondEdition)
            {
                return ship.ActionBar.HasAction(typeof(SlamAction));
            }
            else
            {
                return true;
            }
        }
    }
}

namespace Abilities
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

        private void CheckSlamAction(GenericAction action)
        {
            if (action is SlamAction)
            {
                if (HostShip.IsBumped)
                {
                    Messages.ShowErrorToHuman("Overlapped another ship: Advanced SLAM is skipped");
                }
                else if (HostShip.IsHitObstacles)
                {
                    Messages.ShowErrorToHuman("Overlapped an obstacle: Advanced SLAM is skipped");
                }
                else
                {
                    RegisterTrigger();
                }
            }
        }

        private void RegisterTrigger()
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

            Selection.ThisShip.AskPerformFreeAction(actionBarActions, Triggers.FinishTrigger);
        }

    }
}

namespace Abilities.SecondEdition
{
    public class AdvancedSlamAbility : Abilities.AdvancedSlamAbility
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
