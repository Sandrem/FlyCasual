using Ship;
using Upgrade;
using ActionsList;

namespace UpgradesList
{ 
    public class AdvancedSlam : GenericUpgrade
    {
        public AdvancedSlam() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Advanced SLAM";
            Cost = 2;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnActionIsPerformed += CheckSlamAction;
        }

        private void CheckSlamAction(GenericAction action)
        {
            if (action is SlamAction)
            {
                if (Host.IsBumped)
                {
                    Messages.ShowErrorToHuman("Overlaped another ship: Advanced SLAM is skipped");
                }
                else if (Host.IsHitObstacles)
                {
                    Messages.ShowErrorToHuman("Overlaped an obstacle: Advanced SLAM is skipped");
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
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = PerfromFreeActionFromUpgradeBar
            });
        }

        private void PerfromFreeActionFromUpgradeBar(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AskPerformFreeAction(Selection.ThisShip.GetAvailablePrintedActionsList(), Triggers.FinishTrigger);
        }

    }
}
