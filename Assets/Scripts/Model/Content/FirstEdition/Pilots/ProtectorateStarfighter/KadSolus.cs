using BoardTools;
using Movement;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class KadSolus : ProtectorateStarfighter
        {
            public KadSolus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kad Solus",
                    6,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KadSolusAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //After you execute a red maneuver, assign 2 focus tokens to your ship.
    public class KadSolusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (CheckAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignTokens);
            }
        }

        protected virtual bool CheckAbility()
        {
            if (HostShip.GetLastManeuverColor() != MovementComplexity.Complex) return false;
            if (Board.IsOffTheBoard(HostShip)) return false;

            return true;
        }

        private void AssignTokens(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignTokens(() => new FocusToken(HostShip), 2, Triggers.FinishTrigger);
        }
    }
}