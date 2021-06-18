using Ship;

namespace Abilities
{
    public class HasAnyShipAtRange : Condition
    {
        private ConditionsBlock Conditions;

        public HasAnyShipAtRange(ConditionsBlock conditions)
        {
            Conditions = conditions;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipAbilityHost == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ConditionArgs newArgs = new ConditionArgs()
                {
                    ShipAbilityHost = args.ShipAbilityHost,
                    ShipToCheck = ship
                };

                if (Conditions.Passed(newArgs)) return true;
            }

            return false;
        }

    }
}
