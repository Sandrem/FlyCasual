namespace Abilities
{
    public class HostHasForceCondition : Condition
    {
        public override bool Passed(ConditionArgs args)
        {
            return args.ShipAbilityHost.State.Force > 0;
        }
    }
}
