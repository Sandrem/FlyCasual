using RuleSets;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class KadSolus : ProtectorateStarfighter, ISecondEditionPilot
        {
            public KadSolus() : base()
            {
                PilotName = "Kad Solus";
                PilotSkill = 6;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.KadSolusAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 50; //TODO

                PilotAbilities.RemoveAll(ability => ability is Abilities.KadSolusAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.KadSolusAbility());
            }
        }
    }
}

namespace Abilities
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

    namespace SecondEdition
    {
        //After you fully execute a red maneuver, gain 2 focus tokens.
        public class KadSolusAbility : Abilities.KadSolusAbility
        {
            protected override bool CheckAbility()
            {
                if (HostShip.IsBumped) return false;

                return base.CheckAbility();
            }
        }
    }
}
