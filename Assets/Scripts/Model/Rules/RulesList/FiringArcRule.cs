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
            RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(attacker, defender);
            if (!shotInfo.InArc)
            {
                if (attacker.Owner.Type == Players.PlayerType.Human) Game.UI.ShowError("Ship is outside your firing arc");
                result = false;
            }
        }

    }
}
