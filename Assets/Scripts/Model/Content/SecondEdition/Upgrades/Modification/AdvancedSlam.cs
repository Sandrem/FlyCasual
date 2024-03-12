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
                    Messages.ShowErrorToHuman("SLAM maneuver overlapped another ship, Advanced SLAM action is skipped");
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

        private void PerfromFreeActionFromUpgradeBar(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = HostShip.GetAvailableActions();
            List<GenericAction> whiteActionBarActionsAsRed = actions
                .Where(n => n.IsInActionBar && n.Color == ActionColor.White)
                .Select(n => n.AsRedAction)
                .ToList();

            Selection.ThisShip.AskPerformFreeAction(
                whiteActionBarActionsAsRed,
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                "After you perform a SLAM action, if you fully executed the maneuver, you may perform a white action on your action bar, treating that action as red",
                HostUpgrade
            );
        }
    }
}