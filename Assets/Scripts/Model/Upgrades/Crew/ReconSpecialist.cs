using ActionsList;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    class ReconSpecialist : GenericUpgrade
    {
        public ReconSpecialist() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Recon Specialist";
            Cost = 3;

            AvatarOffset = new Vector2(42, 3);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnActionIsPerformed += CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is FocusAction)
            {
                Host.OnActionDecisionSubphaseEnd += RegisterTrigger;
            }
        }

        private void RegisterTrigger(Ship.GenericShip ship)
        {
            Host.OnActionDecisionSubphaseEnd -= RegisterTrigger;

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
            Host.Tokens.AssignToken(new Tokens.FocusToken(Host), Triggers.FinishTrigger);
        }
    }
}
