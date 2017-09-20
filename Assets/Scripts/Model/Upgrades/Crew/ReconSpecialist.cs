using ActionsList;
using Upgrade;

namespace UpgradesList
{
    class ReconSpecialist : GenericUpgrade
    {
        public ReconSpecialist() : base()
        {
            Type = UpgradeType.Crew;
            Name = ShortName = "Recon Specialist";
            Cost = 3;
        }
        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnActionIsPerformed += ReconSpecialistAbility;
        }

        private void ReconSpecialistAbility(GenericAction action)
        {
            if (action is FocusAction)
                Host.AssignToken(new Tokens.FocusToken(), delegate { });
        }
    }
}
