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
            Actions.OnCheckCanPerformAttack += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            if (!Actions.InArcCheck(attacker, defender))
            {
                Game.UI.ShowError("Ship is outside your firing arc");
                result = false;
            }
        }

    }
}
