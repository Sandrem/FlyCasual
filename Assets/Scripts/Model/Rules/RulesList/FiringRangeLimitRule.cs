namespace RulesList

{
    public class FiringRangeLimit
    {

        public FiringRangeLimit()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
        }

        public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
        {

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(attacker, defender);
            if (shotInfo.Range > 3)
            {
                if (attacker.Owner.Type == Players.PlayerType.Human) Messages.ShowErrorToHuman("Ship is outside your firing range");
                result = false;
            }
        }

    }
}
