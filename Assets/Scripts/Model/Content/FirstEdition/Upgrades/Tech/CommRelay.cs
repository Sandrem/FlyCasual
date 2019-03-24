using Ship;
using Upgrade;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class CommRelay : GenericUpgrade
    {
        public CommRelay() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Comm Relay",
                UpgradeType.Tech,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.CommRelayAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class CommRelayAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeTokenIsAssigned += CommRelayRestriction;
            HostShip.OnTokenIsAssigned += CommRelayBonus;

            ToggleBonusOfExistingEvadeTokens(true);
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeTokenIsAssigned -= CommRelayRestriction;
            HostShip.OnTokenIsAssigned -= CommRelayBonus;

            ToggleBonusOfExistingEvadeTokens(false);
        }

        private void CommRelayRestriction(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(EvadeToken) && ship.Tokens.HasToken(typeof(EvadeToken)))
            {
                Messages.ShowError("Comm Relay: This ship cannot have more than 1 Evade token.");
                ship.Tokens.TokenToAssign = null;
            }
        }

        private void CommRelayBonus(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(EvadeToken))
            {
                ToggleBonusOfExistingEvadeTokens(true);
            }
        }

        private void ToggleBonusOfExistingEvadeTokens(bool isActive)
        {
            EvadeToken evadeToken = (EvadeToken)HostShip.Tokens.GetToken(typeof(EvadeToken));
            if (evadeToken != null)
            {
                evadeToken.Temporary = !isActive;
            }
        }
    }
}
