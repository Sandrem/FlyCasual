namespace RulesList
{
    public class FiringArcRule
    {
        private GameManagerScript Game;

        public FiringArcRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Actions.OnCheckTargetIsLegal += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (!Actions.InArcCheck(attacker, defender))
            {
                if (attacker.Owner.Type == Players.PlayerType.Human) Game.UI.ShowError("Ship is outside your firing arc");
                result = false;
            }
        }

    }
}
