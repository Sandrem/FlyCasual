using Ship;
using Ship.TIEPhantom;
using Upgrade;
using ActionsList;
using System.Collections.Generic;

namespace UpgradesList
{
    public class AdvancedCloakingDevice : GenericUpgrade
    {
        public AdvancedCloakingDevice() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Advanced Cloaking Device";
            Cost = 4;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEPhantom;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackFinish += RegisterPerformFreeCloakAction;
        }

        private void RegisterPerformFreeCloakAction(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Advanced Cloaking Device",
                TriggerType = TriggerTypes.OnAttackFinish,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = PerformFreeCloakAction
            });
        }

        private void PerformFreeCloakAction(object sender, System.EventArgs e)
        {
            List<GenericAction> actions = new List<GenericAction>() { new CloakAction() };

            Host.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}

