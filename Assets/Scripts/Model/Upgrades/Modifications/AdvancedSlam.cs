using Ship;
using Upgrade;
using ActionsList;
using Abilities;

namespace UpgradesList
{
    public class AdvancedSlam : GenericUpgrade
    {
        public AdvancedSlam() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Advanced SLAM";
            Cost = 2;

            UpgradeAbilities.Add(new AdvancedSlamAbility());
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

        private void PerfromFreeActionFromUpgradeBar(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AskPerformFreeAction(Selection.ThisShip.GetAvailablePrintedActionsList(), Triggers.FinishTrigger);
        }
    }
}
