using Ship;
using Ship.HWK290;
using Upgrade;

namespace UpgradesList
{
    public class MoldyCrow : GenericUpgrade
    {
        public MoldyCrow() : base()
        {
            Type = UpgradeType.Title;
            Name = ShortName = "Moldy Crow";
            Cost = 3;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is HWK290;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.BeforeRemovingTokenInEndPhase += KeepFocusTokens;
        }

        private void KeepFocusTokens(Tokens.GenericToken token, ref bool remove)
        {
            if (token is Tokens.FocusToken) remove = false;
        } 
    }
}
