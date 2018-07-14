using Ship;
using Ship.HWK290;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class MoldyCrow : GenericUpgrade
    {
        public MoldyCrow() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Moldy Crow";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new MoldyCrowAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is HWK290;
        }
    }
}

namespace Abilities
{
    public class MoldyCrowAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase += KeepFocusTokens;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase -= KeepFocusTokens;
        }

        private void KeepFocusTokens(Tokens.GenericToken token, ref bool remove)
        {
            if (token is Tokens.FocusToken) remove = false;
        }
    }
}
