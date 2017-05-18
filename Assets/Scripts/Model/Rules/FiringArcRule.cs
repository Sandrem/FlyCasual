namespace Rules
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
            Game.Actions.OnCheckCanPerformAttack += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (!Game.Actions.InArcCheck(attacker, defender))
            {
                Game.UI.ShowError("Ship is outside your firing arc");
                result = false;
            }
        }

    }
}
