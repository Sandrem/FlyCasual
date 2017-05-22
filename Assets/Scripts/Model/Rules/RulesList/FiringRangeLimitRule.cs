namespace RulesList
{
    public class FiringRangeLimit
    {
        private GameManagerScript Game;

        public FiringRangeLimit(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Actions.OnCheckCanPerformAttack += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (Actions.GetRange(attacker, defender) > 3)
            {
                if (attacker.Owner.Type == Players.PlayerType.Human) Game.UI.ShowError("Ship is outside your firing range");
                result = false;
            }
        }

    }
}
