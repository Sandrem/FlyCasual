namespace Abilities
{
    public class CanSpendForceCondition : Condition
    {
        public override bool Passed(ConditionArgs args)
        {
            return args.ShipAbilityHost.State.Force > 0;
        }
    }
}
