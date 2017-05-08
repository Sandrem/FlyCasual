namespace Rules
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
            Game.Actions.OnCheckCanPerformAttack += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (Game.Actions.GetRange(attacker, defender) > 3)
            {
                Game.UI.ShowError("Ship is outside your firing range");
                result = false;
            }
        }

    }
}
