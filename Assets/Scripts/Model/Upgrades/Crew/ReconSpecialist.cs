using ActionsList;
using Upgrade;

namespace UpgradesList
{
    class ReconSpecialist : GenericUpgrade
    {
        public ReconSpecialist() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Recon Specialist";
            Cost = 3;
        }
        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnActionIsPerformed += RegisterTrigger;
       } 

        private void RegisterTrigger(GenericAction action)
        {
            if (action is FocusAction)
                Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Recon Specialist's ability",
                        TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                        TriggerOwner = Host.Owner.PlayerNo,
                        EventHandler = ReconSpecialistAbility
                    });
        }

        private void ReconSpecialistAbility(object sender, System.EventArgs e)
        {
            Host.AssignToken(new Tokens.FocusToken(), Triggers.FinishTrigger);
        }
    }
}
