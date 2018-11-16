using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class KadSolus : FangFighter
        {
            public KadSolus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kad Solus",
                    4,
                    54,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KadSolusAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 158;
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

        private void RegisterTrigger(Ship.GenericShip ship)
        {
            if (CheckAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignTokens);
            }
        }

        protected virtual bool CheckAbility()
        {
            if (HostShip.GetLastManeuverColor() != Movement.MovementComplexity.Complex) return false;
            if (BoardTools.Board.IsOffTheBoard(HostShip)) return false;

            return true;
        }

        private void AssignTokens(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignTokens(() => new Tokens.FocusToken(HostShip), 2, Triggers.FinishTrigger);
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, gain 2 focus tokens.
    public class KadSolusAbility : Abilities.FirstEdition.KadSolusAbility
    {
        protected override bool CheckAbility()
        {
            if (HostShip.IsBumped) return false;

            return base.CheckAbility();
        }
    }
}